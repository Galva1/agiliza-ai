using HelpDesk.Api.Common.Exceptions;
using HelpDesk.Api.Common.Mappings;
using HelpDesk.Api.Data;
using HelpDesk.Api.DTOs.Users;
using HelpDesk.Api.Entities;
using HelpDesk.Api.Entities.Enums;
using HelpDesk.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Api.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        return await _db.Usuarios
            .OrderBy(u => u.Nome)
            .Select(u => u.ToDto())
            .ToListAsync();
    }

    public async Task<UserDto> GetByIdAsync(Guid id)
    {
        var user = await FindUserOrThrowAsync(id);
        return user.ToDto();
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request, Guid createdByUserId)
    {
        var email = request.Email.Trim().ToLower();

        var emailInUse = await _db.Usuarios.AnyAsync(u => u.Email == email);
        if (emailInUse)
        {
            throw new BusinessRuleException("Já existe um usuário com este e-mail.");
        }

        var user = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = request.Name.Trim(),
            Email = email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Perfil = request.Role,
            Ativo = true,
            DataCriacao = DateTime.UtcNow,
            CriadoPorId = createdByUserId
        };

        _db.Usuarios.Add(user);
        await _db.SaveChangesAsync();

        return user.ToDto();
    }

    public async Task<UserDto> UpdateRoleAsync(Guid id, Perfil role, Guid updatedByUserId)
    {
        var user = await FindUserOrThrowAsync(id);
        user.Perfil = role;
        user.DataAlteracao = DateTime.UtcNow;
        user.AlteradoPorId = updatedByUserId;
        await _db.SaveChangesAsync();
        return user.ToDto();
    }

    public async Task<UserDto> UpdateStatusAsync(Guid id, bool isActive, Guid updatedByUserId)
    {
        var user = await FindUserOrThrowAsync(id);
        user.Ativo = isActive;
        user.DataAlteracao = DateTime.UtcNow;
        user.AlteradoPorId = updatedByUserId;
        await _db.SaveChangesAsync();
        return user.ToDto();
    }

    public async Task<UserDto> UpdateMyProfileAsync(Guid id, UpdateMyProfileRequest request)
    {
        var user = await FindUserOrThrowAsync(id);
        user.Nome = request.Name.Trim();

        if (!string.IsNullOrWhiteSpace(request.NewPassword))
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword) ||
                !BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.SenhaHash))
            {
                throw new BusinessRuleException("Senha atual inválida.");
            }

            user.SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        }

        user.DataAlteracao = DateTime.UtcNow;
        user.AlteradoPorId = id;
        await _db.SaveChangesAsync();
        return user.ToDto();
    }

    public async Task<List<UserDto>> GetAssignableAgentsAsync()
    {
        return await _db.Usuarios
            .Where(u => u.Ativo && (u.Perfil == Perfil.Agente || u.Perfil == Perfil.Admin))
            .OrderBy(u => u.Nome)
            .Select(u => u.ToDto())
            .ToListAsync();
    }

    private async Task<Usuario> FindUserOrThrowAsync(Guid id)
    {
        var user = await _db.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
        {
            throw new NotFoundException("Usuário não encontrado.");
        }
        return user;
    }
}
