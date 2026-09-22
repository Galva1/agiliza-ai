using System.ComponentModel.DataAnnotations;
using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.DTOs.Chamados;

public class CreateChamadoRequest
{
    [Required, MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    public string Descricao { get; set; } = string.Empty;

    public PrioridadeChamado Prioridade { get; set; } = PrioridadeChamado.Media;

    public Guid? CategoriaId { get; set; }
}
