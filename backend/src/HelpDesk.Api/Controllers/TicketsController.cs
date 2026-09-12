using HelpDesk.Api.Common.Extensions;
using HelpDesk.Api.DTOs.Tickets;
using HelpDesk.Api.Entities.Enums;
using HelpDesk.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Api.Controllers;

[ApiController]
[Route("api/tickets")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TicketDto>>> GetAll([FromQuery] TicketStatus? status)
    {
        var userId = User.GetUserId();
        var role = User.GetUserRole();
        return Ok(await _ticketService.GetForUserAsync(userId, role, status));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TicketDetailDto>> GetById(Guid id)
    {
        var userId = User.GetUserId();
        var role = User.GetUserRole();
        return Ok(await _ticketService.GetByIdAsync(id, userId, role));
    }

    [HttpPost]
    public async Task<ActionResult<TicketDto>> Create([FromBody] CreateTicketRequest request)
    {
        var userId = User.GetUserId();
        var ticket = await _ticketService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TicketDto>> Update(Guid id, [FromBody] UpdateTicketRequest request)
    {
        var userId = User.GetUserId();
        var role = User.GetUserRole();
        return Ok(await _ticketService.UpdateAsync(id, request, userId, role));
    }

    [HttpPost("{id:guid}/comments")]
    public async Task<ActionResult<TicketCommentDto>> AddComment(Guid id, [FromBody] CreateCommentRequest request)
    {
        var userId = User.GetUserId();
        var role = User.GetUserRole();
        var comment = await _ticketService.AddCommentAsync(id, request, userId, role);
        return Ok(comment);
    }
}
