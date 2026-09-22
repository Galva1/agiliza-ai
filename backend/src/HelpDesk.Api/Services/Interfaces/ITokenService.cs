using HelpDesk.Api.Entities;

namespace HelpDesk.Api.Services.Interfaces;

public interface ITokenService
{
<<<<<<< HEAD
    (string Token, DateTime ExpiresAt) GenerateToken(Usuario usuario);
=======
    (string Token, DateTime ExpiresAt) GenerateToken(Usuario user);
>>>>>>> 974e11b1c3aaa77e243ec890700622933fdbd10d
}
