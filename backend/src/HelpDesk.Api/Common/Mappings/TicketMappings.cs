using HelpDesk.Api.DTOs.Tickets;
using HelpDesk.Api.Entities;

namespace HelpDesk.Api.Common.Mappings;

public static class TicketMappings
{
    public static TicketDto ToDto(this Chamado ticket) => new()
    {
        Id = ticket.Id,
        Title = ticket.Titulo,
        Description = ticket.Descricao,
        Status = ticket.Status?.Nome ?? string.Empty,
        Priority = ticket.Prioridade,
        RequesterId = ticket.SolicitanteId,
        RequesterName = ticket.Solicitante?.Nome ?? string.Empty,
        AssigneeId = ticket.ResponsavelId,
        AssigneeName = ticket.Responsavel?.Nome,
        CreatedAt = ticket.DataCriacao,
        UpdatedAt = ticket.DataAlteracao,
        ClosedAt = ticket.DataFechamento
    };

    public static TicketDetailDto ToDetailDto(this Chamado ticket) => new()
    {
        Id = ticket.Id,
        Title = ticket.Titulo,
        Description = ticket.Descricao,
        Status = ticket.Status?.Nome ?? string.Empty,
        Priority = ticket.Prioridade,
        RequesterId = ticket.SolicitanteId,
        RequesterName = ticket.Solicitante?.Nome ?? string.Empty,
        AssigneeId = ticket.ResponsavelId,
        AssigneeName = ticket.Responsavel?.Nome,
        CreatedAt = ticket.DataCriacao,
        UpdatedAt = ticket.DataAlteracao,
        ClosedAt = ticket.DataFechamento,
        Comments = ticket.Comentarios
            .OrderBy(c => c.DataCriacao)
            .Select(c => c.ToDto())
            .ToList()
    };

    public static TicketCommentDto ToDto(this ChamadoComentario comment) => new()
    {
        Id = comment.Id,
        UserId = comment.AutorId,
        UserName = comment.Autor?.Nome ?? string.Empty,
        Message = comment.Mensagem,
        CreatedAt = comment.DataCriacao
    };

    public static TicketStatusDto ToDto(this StatusChamado status) => new()
    {
        Id = status.Id,
        Name = status.Nome
    };
}
