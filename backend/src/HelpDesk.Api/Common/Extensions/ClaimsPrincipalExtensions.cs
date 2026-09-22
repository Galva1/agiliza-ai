using System.Security.Claims;
using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(value!);
    }

<<<<<<< HEAD
    public static PerfilUsuario GetUserPerfil(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.Role);
        return Enum.Parse<PerfilUsuario>(value!);
=======
    public static Perfil GetUserRole(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.Role);
        return Enum.Parse<Perfil>(value!);
>>>>>>> 974e11b1c3aaa77e243ec890700622933fdbd10d
    }
}
