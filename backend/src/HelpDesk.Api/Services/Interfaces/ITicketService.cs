using HelpDesk.Api.DTOs.Tickets;
using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.Services.Interfaces;

public interface ITicketService
{
    Task<List<TicketDto>> GetForUserAsync(Guid userId, UserRole role, TicketStatus? status);
    Task<TicketDetailDto> GetByIdAsync(Guid ticketId, Guid userId, UserRole role);
    Task<TicketDto> CreateAsync(CreateTicketRequest request, Guid requesterId);
    Task<TicketDto> UpdateAsync(Guid ticketId, UpdateTicketRequest request, Guid userId, UserRole role);
    Task<TicketCommentDto> AddCommentAsync(Guid ticketId, CreateCommentRequest request, Guid userId, UserRole role);
}
