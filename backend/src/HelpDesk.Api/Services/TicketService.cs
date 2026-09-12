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

    public async Task<List<TicketDto>> GetForUserAsync(Guid userId, Perfil role, string? statusName)
    {
        var query = _db.Chamados
            .Include(t => t.Solicitante)
            .Include(t => t.Responsavel)
            .Include(t => t.Status)
            .AsQueryable();

        if (role == Perfil.Solicitante)
        {
            query = query.Where(t => t.SolicitanteId == userId);
        }

        if (!string.IsNullOrWhiteSpace(statusName))
        {
            query = query.Where(t => t.Status!.Nome == statusName);
        }

        return await query
            .OrderByDescending(t => t.DataCriacao)
            .Select(t => t.ToDto())
            .ToListAsync();
    }

    public async Task<TicketDetailDto> GetByIdAsync(Guid ticketId, Guid userId, Perfil role)
    {
        var ticket = await FindTicketOrThrowAsync(ticketId, includeComments: true);
        EnsureCanView(ticket, userId, role);
        return ticket.ToDetailDto();
    }

    public async Task<TicketDto> CreateAsync(CreateTicketRequest request, Guid requesterId)
    {
        var abertoStatusId = await GetStatusIdByNameAsync(StatusChamadoNomes.Aberto);

        var ticket = new Chamado
        {
            Id = Guid.NewGuid(),
            Titulo = request.Title.Trim(),
            Descricao = request.Description.Trim(),
            Prioridade = request.Priority,
            StatusChamadoId = abertoStatusId,
            SolicitanteId = requesterId,
            Ativo = true,
            DataCriacao = DateTime.UtcNow,
            CriadoPorId = requesterId
        };

        _db.Chamados.Add(ticket);
        await _db.SaveChangesAsync();

        await _db.Entry(ticket).Reference(t => t.Solicitante).LoadAsync();
        await _db.Entry(ticket).Reference(t => t.Status).LoadAsync();

        return ticket.ToDto();
    }

    public async Task<TicketDto> UpdateAsync(Guid ticketId, UpdateTicketRequest request, Guid userId, Perfil role)
    {
        if (role == Perfil.Solicitante)
        {
            throw new ForbiddenException("Solicitantes não podem alterar chamados.");
        }

        var ticket = await FindTicketOrThrowAsync(ticketId, includeComments: false);
        var hasChanges = false;

        if (request.AssigneeId.HasValue)
        {
            var assigneeExists = await _db.Usuarios.AnyAsync(u =>
                u.Id == request.AssigneeId.Value &&
                u.Ativo &&
                (u.Perfil == Perfil.Agente || u.Perfil == Perfil.Admin));

            if (!assigneeExists)
            {
                throw new BusinessRuleException("Responsável inválido. Selecione um agente ativo.");
            }

            ticket.ResponsavelId = request.AssigneeId.Value;
            hasChanges = true;
        }

        if (request.Priority.HasValue)
        {
            ticket.Prioridade = request.Priority.Value;
            hasChanges = true;
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            ticket.StatusChamadoId = await GetStatusIdByNameAsync(request.Status);
            ticket.DataFechamento = request.Status == StatusChamadoNomes.Fechado
                ? DateTime.UtcNow
                : null;
            hasChanges = true;
        }

        if (hasChanges)
        {
            ticket.DataAlteracao = DateTime.UtcNow;
            ticket.AlteradoPorId = userId;
            await _db.SaveChangesAsync();
        }

        await _db.Entry(ticket).Reference(t => t.Solicitante).LoadAsync();
        await _db.Entry(ticket).Reference(t => t.Status).LoadAsync();
        if (ticket.ResponsavelId.HasValue)
        {
            await _db.Entry(ticket).Reference(t => t.Responsavel).LoadAsync();
        }

        return ticket.ToDto();
    }

    public async Task<TicketCommentDto> AddCommentAsync(Guid ticketId, CreateCommentRequest request, Guid userId, Perfil role)
    {
        var ticket = await FindTicketOrThrowAsync(ticketId, includeComments: false);
        EnsureCanView(ticket, userId, role);

        var comment = new ChamadoComentario
        {
            Id = Guid.NewGuid(),
            ChamadoId = ticketId,
            AutorId = userId,
            Mensagem = request.Message.Trim(),
            Ativo = true,
            DataCriacao = DateTime.UtcNow
        };

        _db.ChamadoComentarios.Add(comment);

        ticket.DataAlteracao = DateTime.UtcNow;
        ticket.AlteradoPorId = userId;

        await _db.SaveChangesAsync();
        await _db.Entry(comment).Reference(c => c.Autor).LoadAsync();

        return comment.ToDto();
    }

    public async Task<List<TicketStatusDto>> GetStatusesAsync()
    {
        return await _db.StatusChamados
            .Where(s => s.Ativo)
            .OrderBy(s => s.Id)
            .Select(s => s.ToDto())
            .ToListAsync();
    }

    private async Task<int> GetStatusIdByNameAsync(string name)
    {
        var status = await _db.StatusChamados.FirstOrDefaultAsync(s => s.Nome == name && s.Ativo);
        if (status is null)
        {
            throw new BusinessRuleException($"Status '{name}' não existe ou está inativo.");
        }
        return status.Id;
    }

    private async Task<Chamado> FindTicketOrThrowAsync(Guid ticketId, bool includeComments)
    {
        var query = _db.Chamados
            .Include(t => t.Solicitante)
            .Include(t => t.Responsavel)
            .Include(t => t.Status)
            .AsQueryable();

        if (includeComments)
        {
            query = query.Include(t => t.Comentarios).ThenInclude(c => c.Autor);
        }

        var ticket = await query.FirstOrDefaultAsync(t => t.Id == ticketId);
        if (ticket is null)
        {
            throw new NotFoundException("Chamado não encontrado.");
        }

        return ticket;
    }

    private static void EnsureCanView(Chamado ticket, Guid userId, Perfil role)
    {
        if (role == Perfil.Solicitante && ticket.SolicitanteId != userId)
        {
            throw new ForbiddenException("Você não tem permissão para acessar este chamado.");
        }
    }
}
