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

    public static Perfil GetUserRole(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.Role);
        return Enum.Parse<Perfil>(value!);
    }
}
