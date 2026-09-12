using HelpDesk.Api.Entities;

namespace HelpDesk.Api.Services.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}
