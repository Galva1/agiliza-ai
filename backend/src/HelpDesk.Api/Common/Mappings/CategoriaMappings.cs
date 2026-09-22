using HelpDesk.Api.DTOs.Categorias;
using HelpDesk.Api.Entities;

namespace HelpDesk.Api.Common.Mappings;

public static class CategoriaMappings
{
    public static CategoriaDto ToDto(this Categoria categoria) => new()
    {
        Id = categoria.Id,
        Nome = categoria.Nome,
        Descricao = categoria.Descricao,
        Ativa = categoria.Ativa,
        CriadoEm = categoria.CriadoEm
    };
}
