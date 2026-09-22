using HelpDesk.Api.DTOs.Categorias;

namespace HelpDesk.Api.Services.Interfaces;

public interface ICategoriaService
{
    Task<List<CategoriaDto>> GetAllAsync(bool somenteAtivas);
    Task<CategoriaDto> CreateAsync(CreateCategoriaRequest request);
    Task<CategoriaDto> UpdateAsync(Guid id, UpdateCategoriaRequest request);
    Task<CategoriaDto> UpdateStatusAsync(Guid id, bool ativa);
}
