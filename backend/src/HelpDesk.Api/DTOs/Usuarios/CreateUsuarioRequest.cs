using System.ComponentModel.DataAnnotations;
using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.DTOs.Usuarios;

public class CreateUsuarioRequest
{
    [Required, MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Senha { get; set; } = string.Empty;

    public PerfilUsuario Perfil { get; set; } = PerfilUsuario.Solicitante;
}
