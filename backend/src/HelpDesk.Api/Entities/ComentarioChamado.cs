using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.Entities;

public class ComentarioChamado
{
    public Guid Id { get; set; }
    public Guid ChamadoId { get; set; }
    public Chamado? Chamado { get; set; }

    public Guid UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public string Mensagem { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public TipoComentario Tipo { get; set; } = TipoComentario.Comentario;

    // Pendência: quem deve resolver.
    public ResponsavelPendencia? Responsavel { get; set; }

    // Pendência e solicitação de aprovação: andamento do registro.
    public SituacaoComentario? Situacao { get; set; }
    public string? Resposta { get; set; }
    public Guid? RespondidoPorId { get; set; }
    public Usuario? RespondidoPor { get; set; }
    public DateTime? RespondidoEm { get; set; }
}
