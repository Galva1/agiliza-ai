using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.Entities;

public class Chamado
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public StatusChamado Status { get; set; } = StatusChamado.Aberto;
    public PrioridadeChamado Prioridade { get; set; } = PrioridadeChamado.Media;

    public Guid? CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    public Guid SolicitanteId { get; set; }
    public Usuario? Solicitante { get; set; }

    public Guid? TecnicoId { get; set; }
    public Usuario? Tecnico { get; set; }

    public string? Resolucao { get; set; }
    public bool ResolucaoAprovada { get; set; }
    public DateTime? ResolucaoPropostaEm { get; set; }
    public DateTime? ConcluidoEm { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public DateTime? FechadoEm { get; set; }

    public ICollection<ComentarioChamado> Comentarios { get; set; } = new List<ComentarioChamado>();
    public ICollection<HistoricoChamado> Historico { get; set; } = new List<HistoricoChamado>();
}
