using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.DTOs.Chamados;

public class ResolucaoChamadoDto
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public SituacaoResolucao Situacao { get; set; }
    public string? MotivoRecusa { get; set; }
    public DateTime CriadoEm { get; set; }
    public string? AvaliadoPorNome { get; set; }
    public DateTime? AvaliadoEm { get; set; }
}
