using HelpDesk.Api.DTOs.Autenticacao;

namespace HelpDesk.Api.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
