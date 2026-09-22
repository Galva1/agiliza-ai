using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.Entities;

public class Usuario
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
<<<<<<< HEAD
    public PerfilUsuario Perfil { get; set; } = PerfilUsuario.Solicitante;
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }

    public ICollection<Chamado> ChamadosSolicitados { get; set; } = new List<Chamado>();
    public ICollection<Chamado> ChamadosAtribuidos { get; set; } = new List<Chamado>();
    public ICollection<ComentarioChamado> Comentarios { get; set; } = new List<ComentarioChamado>();
=======
    public Perfil Perfil { get; set; } = Perfil.Solicitante;
    public bool Ativo { get; set; } = true;

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public Guid? CriadoPorId { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public Guid? AlteradoPorId { get; set; }

    public ICollection<Chamado> ChamadosSolicitados { get; set; } = new List<Chamado>();
    public ICollection<Chamado> ChamadosAtribuidos { get; set; } = new List<Chamado>();
    public ICollection<ChamadoComentario> Comentarios { get; set; } = new List<ChamadoComentario>();
>>>>>>> 974e11b1c3aaa77e243ec890700622933fdbd10d
}
