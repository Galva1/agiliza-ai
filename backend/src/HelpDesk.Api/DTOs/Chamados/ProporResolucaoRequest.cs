using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Api.DTOs.Chamados;

public class ProporResolucaoRequest
{
    [Required]
    public string Resolucao { get; set; } = string.Empty;
}
