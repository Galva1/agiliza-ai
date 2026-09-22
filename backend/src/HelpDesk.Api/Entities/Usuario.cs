using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.Entities;

public class Usuario
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; } = PerfilUsuario.Solicitante;
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }

    public ICollection<Chamado> ChamadosSolicitados { get; set; } = new List<Chamado>();
    public ICollection<Chamado> ChamadosAtribuidos { get; set; } = new List<Chamado>();
    public ICollection<ComentarioChamado> Comentarios { get; set; } = new List<ComentarioChamado>();
}
