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
<<<<<<< HEAD
        var usuario = await _db.Usuarios
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());

        if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
=======
        var user = await _db.Usuarios
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.SenhaHash))
>>>>>>> 974e11b1c3aaa77e243ec890700622933fdbd10d
        {
            throw new AuthenticationException("E-mail ou senha inválidos.");
        }

<<<<<<< HEAD
        if (!usuario.Ativo)
=======
        if (!user.Ativo)
>>>>>>> 974e11b1c3aaa77e243ec890700622933fdbd10d
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
