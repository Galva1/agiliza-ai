using HelpDesk.Api.Common.Exceptions;
using HelpDesk.Api.Common.Mappings;
using HelpDesk.Api.Data;
using HelpDesk.Api.DTOs.Categorias;
using HelpDesk.Api.Entities;
using HelpDesk.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Api.Services;

public class CategoriaService : ICategoriaService
{
    private readonly AppDbContext _db;

    public CategoriaService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CategoriaDto>> GetAllAsync(bool somenteAtivas)
    {
        var query = _db.Categorias.AsQueryable();

        if (somenteAtivas)
        {
            query = query.Where(c => c.Ativa);
        }

        return await query
            .OrderBy(c => c.Nome)
            .Select(c => c.ToDto())
            .ToListAsync();
    }

    public async Task<CategoriaDto> CreateAsync(CreateCategoriaRequest request)
    {
        var nomeEmUso = await _db.Categorias.AnyAsync(c => c.Nome == request.Nome.Trim());
        if (nomeEmUso)
        {
            throw new BusinessRuleException("Já existe uma categoria com este nome.");
        }

        var categoria = new Categoria
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome.Trim(),
            Descricao = request.Descricao?.Trim(),
            Ativa = true,
            CriadoEm = DateTime.UtcNow
        };

        _db.Categorias.Add(categoria);
        await _db.SaveChangesAsync();

        return categoria.ToDto();
    }

    public async Task<CategoriaDto> UpdateAsync(Guid id, UpdateCategoriaRequest request)
    {
        var categoria = await FindCategoriaOrThrowAsync(id);
        categoria.Nome = request.Nome.Trim();
        categoria.Descricao = request.Descricao?.Trim();
        await _db.SaveChangesAsync();
        return categoria.ToDto();
    }

    public async Task<CategoriaDto> UpdateStatusAsync(Guid id, bool ativa)
    {
        var categoria = await FindCategoriaOrThrowAsync(id);
        categoria.Ativa = ativa;
        await _db.SaveChangesAsync();
        return categoria.ToDto();
    }

    private async Task<Categoria> FindCategoriaOrThrowAsync(Guid id)
    {
        var categoria = await _db.Categorias.FirstOrDefaultAsync(c => c.Id == id);
        if (categoria is null)
        {
            throw new NotFoundException("Categoria não encontrada.");
        }
        return categoria;
    }
}
