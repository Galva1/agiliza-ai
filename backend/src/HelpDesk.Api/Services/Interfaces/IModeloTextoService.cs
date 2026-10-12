using HelpDesk.Api.DTOs.ModelosTexto;

namespace HelpDesk.Api.Services.Interfaces;

public interface IModeloTextoService
{
    Task<List<ModeloTextoDto>> GetAllAsync(bool somenteAtivos);
    Task<ModeloTextoDto> CreateAsync(SalvarModeloTextoRequest request);
    Task<ModeloTextoDto> UpdateAsync(Guid id, SalvarModeloTextoRequest request);
    Task<ModeloTextoDto> UpdateStatusAsync(Guid id, bool ativo);
    Task DeleteAsync(Guid id);
}
