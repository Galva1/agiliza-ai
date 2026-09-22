namespace HelpDesk.Api.DTOs.Chamados;

public class HistoricoChamadoDto
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;
    public string Acao { get; set; } = string.Empty;
    public string? ValorAnterior { get; set; }
    public string? ValorNovo { get; set; }
    public DateTime CriadoEm { get; set; }
}
