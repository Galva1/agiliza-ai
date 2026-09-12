using System.ComponentModel.DataAnnotations;
using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.DTOs.Tickets;

public class CreateTicketRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public TicketPriority Priority { get; set; } = TicketPriority.Media;
}
