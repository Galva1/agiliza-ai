namespace HelpDesk.Api.Entities;

/// <summary>
/// Tabela de domínio (ticket_status) em vez de enum: permite renomear ou
/// adicionar status sem precisar alterar código/deploy.
/// </summary>
public class StatusChamado
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}

public static class StatusChamadoNomes
{
    public const string Aberto = "Aberto";
    public const string EmAndamento = "EmAndamento";
    public const string Aguardando = "Aguardando";
    public const string Resolvido = "Resolvido";
    public const string Fechado = "Fechado";
}
