using HelpDesk.Api.Common.Extensions;
using HelpDesk.Api.DTOs.Chamados;
using HelpDesk.Api.Entities.Enums;
using HelpDesk.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Api.Controllers;

[ApiController]
[Route("api/chamados")]
[Authorize]
public class ChamadosController : ControllerBase
{
    private readonly IChamadoService _chamadoService;

    public ChamadosController(IChamadoService chamadoService)
    {
        _chamadoService = chamadoService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ChamadoDto>>> GetAll(
        [FromQuery] StatusChamado? status,
        [FromQuery] bool? semAtribuicao,
        [FromQuery] Guid? categoriaId)
    {
        var userId = User.GetUserId();
        var perfil = User.GetUserPerfil();
        return Ok(await _chamadoService.GetForUserAsync(userId, perfil, status, semAtribuicao, categoriaId));
    }

    [HttpGet("painel")]
    public async Task<ActionResult<PainelChamadosDto>> GetPainel(
        [FromQuery] int tamanhoPagina = 10,
        [FromQuery] int paginaAbertos = 1,
        [FromQuery] int paginaAtribuidos = 1,
        [FromQuery] int paginaConcluidos = 1)
    {
        var userId = User.GetUserId();
        var perfil = User.GetUserPerfil();
        return Ok(await _chamadoService.GetPainelAsync(userId, perfil, tamanhoPagina, paginaAbertos, paginaAtribuidos, paginaConcluidos));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ChamadoDetalheDto>> GetById(Guid id)
    {
        var userId = User.GetUserId();
        var perfil = User.GetUserPerfil();
        return Ok(await _chamadoService.GetByIdAsync(id, userId, perfil));
    }

    [HttpPost]
    public async Task<ActionResult<ChamadoDto>> Create([FromBody] CreateChamadoRequest request)
    {
        var userId = User.GetUserId();
        var chamado = await _chamadoService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = chamado.Id }, chamado);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ChamadoDto>> Update(Guid id, [FromBody] UpdateChamadoRequest request)
    {
        var userId = User.GetUserId();
        var perfil = User.GetUserPerfil();
        return Ok(await _chamadoService.UpdateAsync(id, request, userId, perfil));
    }

    [HttpPost("{id:guid}/comentarios")]
    public async Task<ActionResult<ComentarioChamadoDto>> AddComentario(Guid id, [FromBody] CreateComentarioRequest request)
    {
        var userId = User.GetUserId();
        var perfil = User.GetUserPerfil();
        var comentario = await _chamadoService.AddComentarioAsync(id, request, userId, perfil);
        return Ok(comentario);
    }

    [HttpPost("{id:guid}/resolucao")]
    public async Task<ActionResult<ChamadoDto>> ProporResolucao(Guid id, [FromBody] ProporResolucaoRequest request)
    {
        var userId = User.GetUserId();
        var perfil = User.GetUserPerfil();
        return Ok(await _chamadoService.ProporResolucaoAsync(id, request, userId, perfil));
    }

    [HttpPost("{id:guid}/resolucao/aprovar")]
    public async Task<ActionResult<ChamadoDto>> AprovarResolucao(Guid id)
    {
        var userId = User.GetUserId();
        var perfil = User.GetUserPerfil();
        return Ok(await _chamadoService.AprovarResolucaoAsync(id, userId, perfil));
    }
}
