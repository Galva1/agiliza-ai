using HelpDesk.Api.DTOs.Users;
using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.Services.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto> GetByIdAsync(Guid id);
    Task<UserDto> CreateAsync(CreateUserRequest request);
    Task<UserDto> UpdateRoleAsync(Guid id, UserRole role);
    Task<UserDto> UpdateStatusAsync(Guid id, bool isActive);
    Task<UserDto> UpdateMyProfileAsync(Guid id, UpdateMyProfileRequest request);
    Task<List<UserDto>> GetAssignableAgentsAsync();
}
