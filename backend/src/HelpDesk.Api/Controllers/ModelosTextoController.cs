using HelpDesk.Api.Common.Extensions;
using HelpDesk.Api.DTOs.ModelosTexto;
using HelpDesk.Api.Entities.Enums;
using HelpDesk.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Api.Controllers;

[ApiController]
[Route("api/modelos-texto")]
[Authorize]
public class ModelosTextoController : ControllerBase
{
    private readonly IModeloTextoService _modeloTextoService;

    public ModelosTextoController(IModeloTextoService modeloTextoService)
    {
        _modeloTextoService = modeloTextoService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ModeloTextoDto>>> GetAll()
    {
        var perfil = User.GetUserPerfil();
        var somenteAtivos = perfil != PerfilUsuario.Administrador;
        return Ok(await _modeloTextoService.GetAllAsync(somenteAtivos));
    }

    [HttpPost]
    [Authorize(Roles = nameof(PerfilUsuario.Administrador))]
    public async Task<ActionResult<ModeloTextoDto>> Create([FromBody] SalvarModeloTextoRequest request)
    {
        return Ok(await _modeloTextoService.CreateAsync(request));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = nameof(PerfilUsuario.Administrador))]
    public async Task<ActionResult<ModeloTextoDto>> Update(Guid id, [FromBody] SalvarModeloTextoRequest request)
    {
        return Ok(await _modeloTextoService.UpdateAsync(id, request));
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = nameof(PerfilUsuario.Administrador))]
    public async Task<ActionResult<ModeloTextoDto>> UpdateStatus(Guid id, [FromBody] UpdateModeloTextoStatusRequest request)
    {
        return Ok(await _modeloTextoService.UpdateStatusAsync(id, request.Ativo));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = nameof(PerfilUsuario.Administrador))]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _modeloTextoService.DeleteAsync(id);
        return NoContent();
    }
}
