using HelpDesk.Api.DTOs.Tickets;
using HelpDesk.Api.Entities;

namespace HelpDesk.Api.Common.Mappings;

public static class TicketMappings
{
    public static TicketDto ToDto(this Ticket ticket) => new()
    {
        Id = ticket.Id,
        Title = ticket.Title,
        Description = ticket.Description,
        Status = ticket.Status,
        Priority = ticket.Priority,
        RequesterId = ticket.RequesterId,
        RequesterName = ticket.Requester?.Name ?? string.Empty,
        AssigneeId = ticket.AssigneeId,
        AssigneeName = ticket.Assignee?.Name,
        CreatedAt = ticket.CreatedAt,
        UpdatedAt = ticket.UpdatedAt,
        ClosedAt = ticket.ClosedAt
    };

    public static TicketDetailDto ToDetailDto(this Ticket ticket) => new()
    {
        Id = ticket.Id,
        Title = ticket.Title,
        Description = ticket.Description,
        Status = ticket.Status,
        Priority = ticket.Priority,
        RequesterId = ticket.RequesterId,
        RequesterName = ticket.Requester?.Name ?? string.Empty,
        AssigneeId = ticket.AssigneeId,
        AssigneeName = ticket.Assignee?.Name,
        CreatedAt = ticket.CreatedAt,
        UpdatedAt = ticket.UpdatedAt,
        ClosedAt = ticket.ClosedAt,
        Comments = ticket.Comments
            .OrderBy(c => c.CreatedAt)
            .Select(c => c.ToDto())
            .ToList()
    };

    public static TicketCommentDto ToDto(this TicketComment comment) => new()
    {
        Id = comment.Id,
        UserId = comment.UserId,
        UserName = comment.User?.Name ?? string.Empty,
        Message = comment.Message,
        CreatedAt = comment.CreatedAt
    };
}
