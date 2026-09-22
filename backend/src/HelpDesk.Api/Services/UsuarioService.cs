using HelpDesk.Api.Common.Exceptions;
using HelpDesk.Api.Common.Mappings;
using HelpDesk.Api.Data;
using HelpDesk.Api.DTOs.Usuarios;
using HelpDesk.Api.Entities;
using HelpDesk.Api.Entities.Enums;
using HelpDesk.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Api.Services;

public class UsuarioService : IUsuarioService
{
    private readonly AppDbContext _db;

    public UsuarioService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<UsuarioDto>> GetAllAsync()
    {
        return await _db.Usuarios
            .OrderBy(u => u.Nome)
            .Select(u => u.ToDto())
            .ToListAsync();
    }

    public async Task<UsuarioDto> GetByIdAsync(Guid id)
    {
        var usuario = await FindUsuarioOrThrowAsync(id);
        return usuario.ToDto();
    }

    public async Task<UsuarioDto> CreateAsync(CreateUsuarioRequest request)
    {
        var email = request.Email.Trim().ToLower();

        var emailInUse = await _db.Usuarios.AnyAsync(u => u.Email == email);
        if (emailInUse)
        {
            throw new BusinessRuleException("Já existe um usuário com este e-mail.");
        }

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome.Trim(),
            Email = email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha),
            Perfil = request.Perfil,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        return usuario.ToDto();
    }

    public async Task<UsuarioDto> UpdatePerfilAsync(Guid id, PerfilUsuario perfil)
    {
        var usuario = await FindUsuarioOrThrowAsync(id);
        usuario.Perfil = perfil;
        usuario.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return usuario.ToDto();
    }

    public async Task<UsuarioDto> UpdateStatusAsync(Guid id, bool ativo)
    {
        var usuario = await FindUsuarioOrThrowAsync(id);
        usuario.Ativo = ativo;
        usuario.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return usuario.ToDto();
    }

    public async Task<UsuarioDto> UpdateMeuPerfilAsync(Guid id, UpdateMeuPerfilRequest request)
    {
        var usuario = await FindUsuarioOrThrowAsync(id);
        usuario.Nome = request.Nome.Trim();

        if (!string.IsNullOrWhiteSpace(request.NovaSenha))
        {
            if (string.IsNullOrWhiteSpace(request.SenhaAtual) ||
                !BCrypt.Net.BCrypt.Verify(request.SenhaAtual, usuario.SenhaHash))
            {
                throw new BusinessRuleException("Senha atual inválida.");
            }

            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha);
        }

        usuario.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return usuario.ToDto();
    }

    public async Task<List<UsuarioDto>> GetTecnicosAtribuiveisAsync()
    {
        return await _db.Usuarios
            .Where(u => u.Ativo && (u.Perfil == PerfilUsuario.Tecnico || u.Perfil == PerfilUsuario.Administrador))
            .OrderBy(u => u.Nome)
            .Select(u => u.ToDto())
            .ToListAsync();
    }

    private async Task<Usuario> FindUsuarioOrThrowAsync(Guid id)
    {
        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        if (usuario is null)
        {
            throw new NotFoundException("Usuário não encontrado.");
        }
        return usuario;
    }
}
