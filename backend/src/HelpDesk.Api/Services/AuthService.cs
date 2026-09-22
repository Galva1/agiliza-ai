using HelpDesk.Api.Common.Exceptions;
using HelpDesk.Api.Common.Mappings;
using HelpDesk.Api.Data;
using HelpDesk.Api.DTOs.Autenticacao;
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
        var usuario = await _db.Usuarios
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());

        if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
        {
            throw new AuthenticationException("E-mail ou senha inválidos.");
        }

        if (!usuario.Ativo)
        {
            throw new AuthenticationException("Usuário inativo. Contate um administrador.");
        }

        var (token, expiresAt) = _tokenService.GenerateToken(usuario);

        return new LoginResponse
        {
            Token = token,
            ExpiraEm = expiresAt,
            Usuario = usuario.ToDto()
        };
    }
}
