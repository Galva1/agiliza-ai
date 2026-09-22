using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Api.DTOs.Categorias;

public class CreateCategoriaRequest
{
    [Required, MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }
}
