using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.Entities;

public class ResolucaoChamado
{
    public Guid Id { get; set; }
    public Guid ChamadoId { get; set; }
    public Chamado? Chamado { get; set; }

    public Guid UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public string Descricao { get; set; } = string.Empty;
    public SituacaoResolucao Situacao { get; set; } = SituacaoResolucao.Pendente;
    public string? MotivoRecusa { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public Guid? AvaliadoPorId { get; set; }
    public Usuario? AvaliadoPor { get; set; }
    public DateTime? AvaliadoEm { get; set; }
}
