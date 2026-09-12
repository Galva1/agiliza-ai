using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Api.DTOs.Tickets;

public class CreateCommentRequest
{
    [Required]
    public string Message { get; set; } = string.Empty;
}
