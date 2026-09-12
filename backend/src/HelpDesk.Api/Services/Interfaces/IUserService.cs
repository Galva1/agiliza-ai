using HelpDesk.Api.DTOs.Users;
using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.Services.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto> GetByIdAsync(Guid id);
    Task<UserDto> CreateAsync(CreateUserRequest request, Guid createdByUserId);
    Task<UserDto> UpdateRoleAsync(Guid id, Perfil role, Guid updatedByUserId);
    Task<UserDto> UpdateStatusAsync(Guid id, bool isActive, Guid updatedByUserId);
    Task<UserDto> UpdateMyProfileAsync(Guid id, UpdateMyProfileRequest request);
    Task<List<UserDto>> GetAssignableAgentsAsync();
}
