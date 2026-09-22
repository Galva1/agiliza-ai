using HelpDesk.Api.DTOs.Chamados;
using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.Services.Interfaces;

public interface IChamadoService
{
    Task<List<ChamadoDto>> GetForUserAsync(Guid userId, PerfilUsuario perfil, StatusChamado? status, bool? semAtribuicao, Guid? categoriaId);
    Task<ChamadoDetalheDto> GetByIdAsync(Guid chamadoId, Guid userId, PerfilUsuario perfil);
    Task<ChamadoDto> CreateAsync(CreateChamadoRequest request, Guid solicitanteId);
    Task<ChamadoDto> UpdateAsync(Guid chamadoId, UpdateChamadoRequest request, Guid userId, PerfilUsuario perfil);
    Task<ComentarioChamadoDto> AddComentarioAsync(Guid chamadoId, CreateComentarioRequest request, Guid userId, PerfilUsuario perfil);
    Task<ChamadoDto> ProporResolucaoAsync(Guid chamadoId, ProporResolucaoRequest request, Guid userId, PerfilUsuario perfil);
    Task<ChamadoDto> AprovarResolucaoAsync(Guid chamadoId, Guid userId, PerfilUsuario perfil);
    Task<PainelChamadosDto> GetPainelAsync(Guid userId, PerfilUsuario perfil, int tamanhoPagina, int paginaAbertos, int paginaAtribuidos, int paginaConcluidos);
}
