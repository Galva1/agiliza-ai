using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Api.DTOs.Chamados;

public class CreateComentarioRequest
{
    [Required]
    public string Mensagem { get; set; } = string.Empty;
}
