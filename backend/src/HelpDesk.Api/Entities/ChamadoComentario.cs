namespace HelpDesk.Api.Entities;

public class ChamadoComentario
{
    public Guid Id { get; set; }
    public Guid ChamadoId { get; set; }
    public Chamado? Chamado { get; set; }

    /// <summary>Autor do comentário. Também representa quem criou o registro (usr_criacao).</summary>
    public Guid AutorId { get; set; }
    public Usuario? Autor { get; set; }

    public string Mensagem { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataAlteracao { get; set; }
    public Guid? AlteradoPorId { get; set; }
}
