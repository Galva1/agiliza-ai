using HelpDesk.Api.DTOs.Usuarios;
using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.Services.Interfaces;

public interface IUsuarioService
{
    Task<List<UsuarioDto>> GetAllAsync();
    Task<UsuarioDto> GetByIdAsync(Guid id);
    Task<UsuarioDto> CreateAsync(CreateUsuarioRequest request);
    Task<UsuarioDto> UpdatePerfilAsync(Guid id, PerfilUsuario perfil);
    Task<UsuarioDto> UpdateStatusAsync(Guid id, bool ativo);
    Task<UsuarioDto> UpdateMeuPerfilAsync(Guid id, UpdateMeuPerfilRequest request);
    Task<List<UsuarioDto>> GetTecnicosAtribuiveisAsync();
}
