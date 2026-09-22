using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Api.DTOs.Categorias;

public class UpdateCategoriaRequest
{
    [Required, MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }
}
