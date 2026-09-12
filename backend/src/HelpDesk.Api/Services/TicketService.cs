using HelpDesk.Api.Common.Exceptions;
using HelpDesk.Api.Common.Mappings;
using HelpDesk.Api.Data;
using HelpDesk.Api.DTOs.Tickets;
using HelpDesk.Api.Entities;
using HelpDesk.Api.Entities.Enums;
using HelpDesk.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Api.Services;

public class TicketService : ITicketService
{
    private readonly AppDbContext _db;

    public TicketService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<TicketDto>> GetForUserAsync(Guid userId, UserRole role, TicketStatus? status)
    {
        var query = _db.Tickets
            .Include(t => t.Requester)
            .Include(t => t.Assignee)
            .AsQueryable();

        if (role == UserRole.Solicitante)
        {
            query = query.Where(t => t.RequesterId == userId);
        }

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => t.ToDto())
            .ToListAsync();
    }

    public async Task<TicketDetailDto> GetByIdAsync(Guid ticketId, Guid userId, UserRole role)
    {
        var ticket = await FindTicketOrThrowAsync(ticketId, includeComments: true);
        EnsureCanView(ticket, userId, role);
        return ticket.ToDetailDto();
    }

    public async Task<TicketDto> CreateAsync(CreateTicketRequest request, Guid requesterId)
    {
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Priority = request.Priority,
            Status = TicketStatus.Aberto,
            RequesterId = requesterId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync();

        await _db.Entry(ticket).Reference(t => t.Requester).LoadAsync();

        return ticket.ToDto();
    }

    public async Task<TicketDto> UpdateAsync(Guid ticketId, UpdateTicketRequest request, Guid userId, UserRole role)
    {
        if (role == UserRole.Solicitante)
        {
            throw new ForbiddenException("Solicitantes não podem alterar chamados.");
        }

        var ticket = await FindTicketOrThrowAsync(ticketId, includeComments: false);

        if (request.AssigneeId.HasValue)
        {
            var assigneeExists = await _db.Users.AnyAsync(u =>
                u.Id == request.AssigneeId.Value &&
                u.IsActive &&
                (u.Role == UserRole.Agente || u.Role == UserRole.Admin));

            if (!assigneeExists)
            {
                throw new BusinessRuleException("Responsável inválido. Selecione um agente ativo.");
            }

            ticket.AssigneeId = request.AssigneeId.Value;
        }

        if (request.Priority.HasValue)
        {
            ticket.Priority = request.Priority.Value;
        }

        if (request.Status.HasValue)
        {
            ticket.Status = request.Status.Value;
            ticket.ClosedAt = request.Status.Value == TicketStatus.Fechado
                ? DateTime.UtcNow
                : null;
        }

        ticket.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        await _db.Entry(ticket).Reference(t => t.Requester).LoadAsync();
        if (ticket.AssigneeId.HasValue)
        {
            await _db.Entry(ticket).Reference(t => t.Assignee).LoadAsync();
        }

        return ticket.ToDto();
    }

    public async Task<TicketCommentDto> AddCommentAsync(Guid ticketId, CreateCommentRequest request, Guid userId, UserRole role)
    {
        var ticket = await FindTicketOrThrowAsync(ticketId, includeComments: false);
        EnsureCanView(ticket, userId, role);

        var comment = new TicketComment
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            UserId = userId,
            Message = request.Message.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _db.TicketComments.Add(comment);

        ticket.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        await _db.Entry(comment).Reference(c => c.User).LoadAsync();

        return comment.ToDto();
    }

    private async Task<Ticket> FindTicketOrThrowAsync(Guid ticketId, bool includeComments)
    {
        var query = _db.Tickets
            .Include(t => t.Requester)
            .Include(t => t.Assignee)
            .AsQueryable();

        if (includeComments)
        {
            query = query.Include(t => t.Comments).ThenInclude(c => c.User);
        }

        var ticket = await query.FirstOrDefaultAsync(t => t.Id == ticketId);
        if (ticket is null)
        {
            throw new NotFoundException("Chamado não encontrado.");
        }

        return ticket;
    }

    private static void EnsureCanView(Ticket ticket, Guid userId, UserRole role)
    {
        if (role == UserRole.Solicitante && ticket.RequesterId != userId)
        {
            throw new ForbiddenException("Você não tem permissão para acessar este chamado.");
        }
    }
}
