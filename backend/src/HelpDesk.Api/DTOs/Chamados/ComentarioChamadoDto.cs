using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.DTOs.Chamados;

public class ComentarioChamadoDto
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
    public TipoComentario Tipo { get; set; }
    public ResponsavelPendencia? Responsavel { get; set; }
    public SituacaoComentario? Situacao { get; set; }
    public string? Resposta { get; set; }
    public string? RespondidoPorNome { get; set; }
    public DateTime? RespondidoEm { get; set; }
}
