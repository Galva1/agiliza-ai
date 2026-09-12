using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.DTOs.Users;

public class UpdateUserRoleRequest
{
    public UserRole Role { get; set; }
}
