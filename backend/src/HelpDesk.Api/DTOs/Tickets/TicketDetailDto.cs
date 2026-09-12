namespace HelpDesk.Api.DTOs.Tickets;

public class TicketDetailDto : TicketDto
{
    public List<TicketCommentDto> Comments { get; set; } = new();
}
