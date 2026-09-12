using HelpDesk.Api.DTOs.Tickets;
using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.Services.Interfaces;

public interface ITicketService
{
    Task<List<TicketDto>> GetForUserAsync(Guid userId, Perfil role, string? statusName);
    Task<TicketDetailDto> GetByIdAsync(Guid ticketId, Guid userId, Perfil role);
    Task<TicketDto> CreateAsync(CreateTicketRequest request, Guid requesterId);
    Task<TicketDto> UpdateAsync(Guid ticketId, UpdateTicketRequest request, Guid userId, Perfil role);
    Task<TicketCommentDto> AddCommentAsync(Guid ticketId, CreateCommentRequest request, Guid userId, Perfil role);
    Task<List<TicketStatusDto>> GetStatusesAsync();
}
