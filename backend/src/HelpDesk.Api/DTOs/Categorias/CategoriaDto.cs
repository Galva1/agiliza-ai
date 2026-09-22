namespace HelpDesk.Api.DTOs.Categorias;

public class CategoriaDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativa { get; set; }
    public DateTime CriadoEm { get; set; }
}
