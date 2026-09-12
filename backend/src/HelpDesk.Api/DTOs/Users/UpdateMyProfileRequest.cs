using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Api.DTOs.Users;

public class UpdateMyProfileRequest
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MinLength(6)]
    public string? NewPassword { get; set; }

    public string? CurrentPassword { get; set; }
}
