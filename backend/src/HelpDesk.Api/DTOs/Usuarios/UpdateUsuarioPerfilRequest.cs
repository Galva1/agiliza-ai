using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.DTOs.Usuarios;

public class UpdateUsuarioPerfilRequest
{
    public PerfilUsuario Perfil { get; set; }
}
