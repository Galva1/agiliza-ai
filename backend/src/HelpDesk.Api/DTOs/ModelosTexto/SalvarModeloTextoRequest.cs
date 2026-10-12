using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Api.DTOs.ModelosTexto;

public class SalvarModeloTextoRequest
{
    [Required, MaxLength(120)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    public string Conteudo { get; set; } = string.Empty;
}
