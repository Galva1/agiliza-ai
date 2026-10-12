using System.ComponentModel.DataAnnotations;
using HelpDesk.Api.Entities.Enums;

namespace HelpDesk.Api.DTOs.Chamados;

public class CreateComentarioRequest
{
    [Required]
    public string Mensagem { get; set; } = string.Empty;

    public TipoComentario Tipo { get; set; } = TipoComentario.Comentario;

    public ResponsavelPendencia? Responsavel { get; set; }
}
