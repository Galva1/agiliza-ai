using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Api.DTOs.Usuarios;

public class UpdateMeuPerfilRequest
{
    [Required, MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [MinLength(6)]
    public string? NovaSenha { get; set; }

    public string? SenhaAtual { get; set; }
}
