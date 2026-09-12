using HelpDesk.Api.DTOs.Auth;

namespace HelpDesk.Api.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
