using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.DTOs.Chamados;

public class UpdateChamadoRequest
{
    public StatusChamado? Status { get; set; }
    public PrioridadeChamado? Prioridade { get; set; }
    public Guid? CategoriaId { get; set; }
    public Guid? TecnicoId { get; set; }
}
