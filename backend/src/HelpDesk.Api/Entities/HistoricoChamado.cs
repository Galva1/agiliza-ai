namespace HelpDesk.Api.Entities;

public class HistoricoChamado
{
    public Guid Id { get; set; }
    public Guid ChamadoId { get; set; }
    public Chamado? Chamado { get; set; }

    public Guid UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public string Acao { get; set; } = string.Empty;
    public string? ValorAnterior { get; set; }
    public string? ValorNovo { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
