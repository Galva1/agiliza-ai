using HelpDesk.Api.Common.Extensions;
using HelpDesk.Api.DTOs.Usuarios;
using HelpDesk.Api.Entities.Enums;
using HelpDesk.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    [Authorize(Roles = nameof(PerfilUsuario.Administrador))]
    public async Task<ActionResult<List<UsuarioDto>>> GetAll()
    {
        return Ok(await _usuarioService.GetAllAsync());
    }

    [HttpGet("tecnicos")]
    [Authorize(Roles = $"{nameof(PerfilUsuario.Administrador)},{nameof(PerfilUsuario.Tecnico)}")]
    public async Task<ActionResult<List<UsuarioDto>>> GetTecnicos()
    {
        return Ok(await _usuarioService.GetTecnicosAtribuiveisAsync());
    }

    [HttpGet("me")]
    public async Task<ActionResult<UsuarioDto>> GetMe()
    {
        var userId = User.GetUserId();
        return Ok(await _usuarioService.GetByIdAsync(userId));
    }

    [HttpPut("me")]
    public async Task<ActionResult<UsuarioDto>> UpdateMe([FromBody] UpdateMeuPerfilRequest request)
    {
        var userId = User.GetUserId();
        return Ok(await _usuarioService.UpdateMeuPerfilAsync(userId, request));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = nameof(PerfilUsuario.Administrador))]
    public async Task<ActionResult<UsuarioDto>> GetById(Guid id)
    {
        return Ok(await _usuarioService.GetByIdAsync(id));
    }

    [HttpPost]
    [Authorize(Roles = nameof(PerfilUsuario.Administrador))]
    public async Task<ActionResult<UsuarioDto>> Create([FromBody] CreateUsuarioRequest request)
    {
        var usuario = await _usuarioService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id:guid}/perfil")]
    [Authorize(Roles = nameof(PerfilUsuario.Administrador))]
    public async Task<ActionResult<UsuarioDto>> UpdatePerfil(Guid id, [FromBody] UpdateUsuarioPerfilRequest request)
    {
        return Ok(await _usuarioService.UpdatePerfilAsync(id, request.Perfil));
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = nameof(PerfilUsuario.Administrador))]
    public async Task<ActionResult<UsuarioDto>> UpdateStatus(Guid id, [FromBody] UpdateUsuarioStatusRequest request)
    {
        return Ok(await _usuarioService.UpdateStatusAsync(id, request.Ativo));
    }
}
