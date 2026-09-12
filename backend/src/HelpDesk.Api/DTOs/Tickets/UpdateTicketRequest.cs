using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.DTOs.Tickets;

public class UpdateTicketRequest
{
    public TicketStatus? Status { get; set; }
    public TicketPriority? Priority { get; set; }
    public Guid? AssigneeId { get; set; }
}
