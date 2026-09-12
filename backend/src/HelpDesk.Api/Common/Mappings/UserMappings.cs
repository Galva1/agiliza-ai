using HelpDesk.Api.DTOs.Users;
using HelpDesk.Api.Entities;

namespace HelpDesk.Api.Common.Mappings;

public static class UserMappings
{
    public static UserDto ToDto(this Usuario user) => new()
    {
        Id = user.Id,
        Name = user.Nome,
        Email = user.Email,
        Role = user.Perfil,
        IsActive = user.Ativo,
        CreatedAt = user.DataCriacao
    };
}
