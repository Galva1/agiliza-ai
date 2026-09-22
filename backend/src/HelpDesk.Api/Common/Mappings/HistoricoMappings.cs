using HelpDesk.Api.DTOs.Chamados;
using HelpDesk.Api.Entities;

namespace HelpDesk.Api.Common.Mappings;

public static class HistoricoMappings
{
    public static HistoricoChamadoDto ToDto(this HistoricoChamado historico) => new()
    {
        Id = historico.Id,
        UsuarioId = historico.UsuarioId,
        UsuarioNome = historico.Usuario?.Nome ?? string.Empty,
        Acao = historico.Acao,
        ValorAnterior = historico.ValorAnterior,
        ValorNovo = historico.ValorNovo,
        CriadoEm = historico.CriadoEm
    };
}
