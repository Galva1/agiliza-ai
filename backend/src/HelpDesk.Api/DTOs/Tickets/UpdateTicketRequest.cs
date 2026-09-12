using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.DTOs.Tickets;

public class UpdateTicketRequest
{
    /// <summary>Nome do status (ver ticket_status), ex.: "EmAndamento".</summary>
    public string? Status { get; set; }
    public Prioridade? Priority { get; set; }
    public Guid? AssigneeId { get; set; }
}
