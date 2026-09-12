using HelpDesk.Api.Common.Extensions;
using HelpDesk.Api.DTOs.Users;
using HelpDesk.Api.Entities.Enums;
using HelpDesk.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Roles = nameof(Perfil.Admin))]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        return Ok(await _userService.GetAllAsync());
    }

    [HttpGet("agents")]
    [Authorize(Roles = $"{nameof(Perfil.Admin)},{nameof(Perfil.Agente)}")]
    public async Task<ActionResult<List<UserDto>>> GetAssignableAgents()
    {
        return Ok(await _userService.GetAssignableAgentsAsync());
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetMe()
    {
        var userId = User.GetUserId();
        return Ok(await _userService.GetByIdAsync(userId));
    }

    [HttpPut("me")]
    public async Task<ActionResult<UserDto>> UpdateMe([FromBody] UpdateMyProfileRequest request)
    {
        var userId = User.GetUserId();
        return Ok(await _userService.UpdateMyProfileAsync(userId, request));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = nameof(Perfil.Admin))]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        return Ok(await _userService.GetByIdAsync(id));
    }

    [HttpPost]
    [Authorize(Roles = nameof(Perfil.Admin))]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserRequest request)
    {
        var user = await _userService.CreateAsync(request, User.GetUserId());
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id:guid}/role")]
    [Authorize(Roles = nameof(Perfil.Admin))]
    public async Task<ActionResult<UserDto>> UpdateRole(Guid id, [FromBody] UpdateUserRoleRequest request)
    {
        return Ok(await _userService.UpdateRoleAsync(id, request.Role, User.GetUserId()));
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = nameof(Perfil.Admin))]
    public async Task<ActionResult<UserDto>> UpdateStatus(Guid id, [FromBody] UpdateUserStatusRequest request)
    {
        return Ok(await _userService.UpdateStatusAsync(id, request.IsActive, User.GetUserId()));
    }
}
