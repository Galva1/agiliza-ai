using HelpDesk.Api.DTOs.Usuarios;
using HelpDesk.Api.Entities;

namespace HelpDesk.Api.Common.Mappings;

public static class UsuarioMappings
{
    public static UsuarioDto ToDto(this Usuario usuario) => new()
    {
        Id = usuario.Id,
        Nome = usuario.Nome,
        Email = usuario.Email,
        Perfil = usuario.Perfil,
        Ativo = usuario.Ativo,
        CriadoEm = usuario.CriadoEm
    };
}
