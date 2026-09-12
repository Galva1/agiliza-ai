using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.DTOs.Users;

public class UpdateUserRoleRequest
{
    public Perfil Role { get; set; }
}
