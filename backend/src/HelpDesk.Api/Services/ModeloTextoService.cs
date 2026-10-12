using HelpDesk.Api.Common.Exceptions;
using HelpDesk.Api.Common.Mappings;
using HelpDesk.Api.Data;
using HelpDesk.Api.DTOs.ModelosTexto;
using HelpDesk.Api.Entities;
using HelpDesk.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Api.Services;

public class ModeloTextoService : IModeloTextoService
{
    private readonly AppDbContext _db;

    public ModeloTextoService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ModeloTextoDto>> GetAllAsync(bool somenteAtivos)
    {
        var query = _db.ModelosTexto.AsQueryable();

        if (somenteAtivos)
        {
            query = query.Where(m => m.Ativo);
        }

        return await query
            .OrderBy(m => m.Titulo)
            .Select(m => m.ToDto())
            .ToListAsync();
    }

    public async Task<ModeloTextoDto> CreateAsync(SalvarModeloTextoRequest request)
    {
        var titulo = request.Titulo.Trim();
        var tituloEmUso = await _db.ModelosTexto.AnyAsync(m => m.Titulo == titulo);
        if (tituloEmUso)
        {
            throw new BusinessRuleException("Já existe um layout com este título.");
        }

        var modelo = new ModeloTexto
        {
            Id = Guid.NewGuid(),
            Titulo = titulo,
            Conteudo = request.Conteudo.Trim(),
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };

        _db.ModelosTexto.Add(modelo);
        await _db.SaveChangesAsync();

        return modelo.ToDto();
    }

    public async Task<ModeloTextoDto> UpdateAsync(Guid id, SalvarModeloTextoRequest request)
    {
        var modelo = await FindModeloOrThrowAsync(id);
        var titulo = request.Titulo.Trim();

        var tituloEmUso = await _db.ModelosTexto.AnyAsync(m => m.Id != id && m.Titulo == titulo);
        if (tituloEmUso)
        {
            throw new BusinessRuleException("Já existe um layout com este título.");
        }

        modelo.Titulo = titulo;
        modelo.Conteudo = request.Conteudo.Trim();
        modelo.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return modelo.ToDto();
    }

    public async Task<ModeloTextoDto> UpdateStatusAsync(Guid id, bool ativo)
    {
        var modelo = await FindModeloOrThrowAsync(id);
        modelo.Ativo = ativo;
        modelo.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return modelo.ToDto();
    }

    public async Task DeleteAsync(Guid id)
    {
        var modelo = await FindModeloOrThrowAsync(id);
        _db.ModelosTexto.Remove(modelo);
        await _db.SaveChangesAsync();
    }

    private async Task<ModeloTexto> FindModeloOrThrowAsync(Guid id)
    {
        var modelo = await _db.ModelosTexto.FirstOrDefaultAsync(m => m.Id == id);
        if (modelo is null)
        {
            throw new NotFoundException("Layout não encontrado.");
        }
        return modelo;
    }
}
