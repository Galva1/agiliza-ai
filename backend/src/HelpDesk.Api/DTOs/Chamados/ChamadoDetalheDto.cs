namespace HelpDesk.Api.DTOs.Chamados;

public class ChamadoDetalheDto : ChamadoDto
{
    public List<ComentarioChamadoDto> Comentarios { get; set; } = new();
    public List<HistoricoChamadoDto> Historico { get; set; } = new();
}
