using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.DTOs.Chamados;

public class ChamadoDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public StatusChamado Status { get; set; }
    public PrioridadeChamado Prioridade { get; set; }

    public Guid? CategoriaId { get; set; }
    public string? CategoriaNome { get; set; }

    public Guid SolicitanteId { get; set; }
    public string SolicitanteNome { get; set; } = string.Empty;

    public Guid? TecnicoId { get; set; }
    public string? TecnicoNome { get; set; }

    public string? Resolucao { get; set; }
    public bool ResolucaoAprovada { get; set; }
    public DateTime? ResolucaoPropostaEm { get; set; }
    public DateTime? ConcluidoEm { get; set; }

    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
    public DateTime? FechadoEm { get; set; }
}
