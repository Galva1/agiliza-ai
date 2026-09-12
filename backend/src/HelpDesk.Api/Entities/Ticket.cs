using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketStatus Status { get; set; } = TicketStatus.Aberto;
    public TicketPriority Priority { get; set; } = TicketPriority.Media;

    public Guid RequesterId { get; set; }
    public User? Requester { get; set; }

    public Guid? AssigneeId { get; set; }
    public User? Assignee { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
}
