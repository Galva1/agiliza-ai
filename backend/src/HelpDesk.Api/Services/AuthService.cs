using HelpDesk.Api.Common.Exceptions;
using HelpDesk.Api.Common.Mappings;
using HelpDesk.Api.Data;
using HelpDesk.Api.DTOs.Auth;
using HelpDesk.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;

    public AuthService(AppDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new AuthenticationException("E-mail ou senha inválidos.");
        }

        if (!user.IsActive)
        {
            throw new AuthenticationException("Usuário inativo. Contate um administrador.");
        }

        var (token, expiresAt) = _tokenService.GenerateToken(user);

        return new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = user.ToDto()
        };
    }
}
