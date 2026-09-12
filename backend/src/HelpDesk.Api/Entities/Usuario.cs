using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.Entities;

public class Usuario
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public Perfil Perfil { get; set; } = Perfil.Solicitante;
    public bool Ativo { get; set; } = true;

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public Guid? CriadoPorId { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public Guid? AlteradoPorId { get; set; }

    public ICollection<Chamado> ChamadosSolicitados { get; set; } = new List<Chamado>();
    public ICollection<Chamado> ChamadosAtribuidos { get; set; } = new List<Chamado>();
    public ICollection<ChamadoComentario> Comentarios { get; set; } = new List<ChamadoComentario>();
}
