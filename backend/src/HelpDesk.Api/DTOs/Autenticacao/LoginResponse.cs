using HelpDesk.Api.DTOs.Usuarios;

namespace HelpDesk.Api.DTOs.Autenticacao;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
    public UsuarioDto Usuario { get; set; } = null!;
}
