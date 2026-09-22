namespace HelpDesk.Api.DTOs.Chamados;

public class ComentarioChamadoDto
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}
