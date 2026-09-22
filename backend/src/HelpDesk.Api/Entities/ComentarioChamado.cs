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
}
