using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.Entities;

public class Chamado
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;

    public int StatusChamadoId { get; set; }
    public StatusChamado? Status { get; set; }

    public Prioridade Prioridade { get; set; } = Prioridade.Media;

    public Guid SolicitanteId { get; set; }
    public Usuario? Solicitante { get; set; }

    public Guid? ResponsavelId { get; set; }
    public Usuario? Responsavel { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public Guid? CriadoPorId { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public Guid? AlteradoPorId { get; set; }
    public DateTime? DataFechamento { get; set; }

    public ICollection<ChamadoComentario> Comentarios { get; set; } = new List<ChamadoComentario>();
}
