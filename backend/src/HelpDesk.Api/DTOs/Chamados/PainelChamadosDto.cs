using HelpDesk.Api.DTOs.Comum;

namespace HelpDesk.Api.DTOs.Chamados;

public class PainelChamadosDto
{
    public PaginaDto<ChamadoDto> Abertos { get; set; } = new();
    public PaginaDto<ChamadoDto> Atribuidos { get; set; } = new();
    public PaginaDto<ChamadoDto> Concluidos { get; set; } = new();
}
