using HelpDesk.Api.DTOs.ModelosTexto;
using HelpDesk.Api.Entities;

namespace HelpDesk.Api.Common.Mappings;

public static class ModeloTextoMappings
{
    public static ModeloTextoDto ToDto(this ModeloTexto modelo) => new()
    {
        Id = modelo.Id,
        Titulo = modelo.Titulo,
        Conteudo = modelo.Conteudo,
        Ativo = modelo.Ativo,
        CriadoEm = modelo.CriadoEm,
        AtualizadoEm = modelo.AtualizadoEm
    };
}
