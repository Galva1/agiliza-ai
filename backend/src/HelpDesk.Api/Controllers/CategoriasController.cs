using HelpDesk.Api.Common.Extensions;
using HelpDesk.Api.DTOs.Categorias;
using HelpDesk.Api.Entities.Enums;
using HelpDesk.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Api.Controllers;

[ApiController]
[Route("api/categorias")]
[Authorize]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _categoriaService;

    public CategoriasController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoriaDto>>> GetAll()
    {
        var perfil = User.GetUserPerfil();
        var somenteAtivas = perfil != PerfilUsuario.Administrador;
        return Ok(await _categoriaService.GetAllAsync(somenteAtivas));
    }

    [HttpPost]
    [Authorize(Roles = nameof(PerfilUsuario.Administrador))]
    public async Task<ActionResult<CategoriaDto>> Create([FromBody] CreateCategoriaRequest request)
    {
        var categoria = await _categoriaService.CreateAsync(request);
        return Ok(categoria);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = nameof(PerfilUsuario.Administrador))]
    public async Task<ActionResult<CategoriaDto>> Update(Guid id, [FromBody] UpdateCategoriaRequest request)
    {
        return Ok(await _categoriaService.UpdateAsync(id, request));
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = nameof(PerfilUsuario.Administrador))]
    public async Task<ActionResult<CategoriaDto>> UpdateStatus(Guid id, [FromBody] UpdateCategoriaStatusRequest request)
    {
        return Ok(await _categoriaService.UpdateStatusAsync(id, request.Ativa));
    }
}
