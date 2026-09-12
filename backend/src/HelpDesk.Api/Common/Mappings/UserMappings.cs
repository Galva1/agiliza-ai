using HelpDesk.Api.DTOs.Users;
using HelpDesk.Api.Entities;

namespace HelpDesk.Api.Common.Mappings;

public static class UserMappings
{
    public static UserDto ToDto(this User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        Role = user.Role,
        IsActive = user.IsActive,
        CreatedAt = user.CreatedAt
    };
}
