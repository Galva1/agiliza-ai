using System.ComponentModel.DataAnnotations;
using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.DTOs.Users;

public class CreateUserRequest
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Solicitante;
}
