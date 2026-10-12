using HelpDesk.Api.DTOs.Chamados;
using HelpDesk.Api.Entities;

namespace HelpDesk.Api.Common.Mappings;

public static class ChamadoMappings
{
    public static ChamadoDto ToDto(this Chamado chamado) => new()
    {
        Id = chamado.Id,
        Titulo = chamado.Titulo,
        Descricao = chamado.Descricao,
        Status = chamado.Status,
        Prioridade = chamado.Prioridade,
        CategoriaId = chamado.CategoriaId,
        CategoriaNome = chamado.Categoria?.Nome,
        SolicitanteId = chamado.SolicitanteId,
        SolicitanteNome = chamado.Solicitante?.Nome ?? string.Empty,
        TecnicoId = chamado.TecnicoId,
        TecnicoNome = chamado.Tecnico?.Nome,
        Resolucao = chamado.Resolucao,
        ResolucaoAprovada = chamado.ResolucaoAprovada,
        ResolucaoPropostaEm = chamado.ResolucaoPropostaEm,
        ConcluidoEm = chamado.ConcluidoEm,
        CriadoEm = chamado.CriadoEm,
        AtualizadoEm = chamado.AtualizadoEm,
        FechadoEm = chamado.FechadoEm
    };

    public static ChamadoDetalheDto ToDetalheDto(this Chamado chamado) => new()
    {
        Id = chamado.Id,
        Titulo = chamado.Titulo,
        Descricao = chamado.Descricao,
        Status = chamado.Status,
        Prioridade = chamado.Prioridade,
        CategoriaId = chamado.CategoriaId,
        CategoriaNome = chamado.Categoria?.Nome,
        SolicitanteId = chamado.SolicitanteId,
        SolicitanteNome = chamado.Solicitante?.Nome ?? string.Empty,
        TecnicoId = chamado.TecnicoId,
        TecnicoNome = chamado.Tecnico?.Nome,
        Resolucao = chamado.Resolucao,
        ResolucaoAprovada = chamado.ResolucaoAprovada,
        ResolucaoPropostaEm = chamado.ResolucaoPropostaEm,
        ConcluidoEm = chamado.ConcluidoEm,
        CriadoEm = chamado.CriadoEm,
        AtualizadoEm = chamado.AtualizadoEm,
        FechadoEm = chamado.FechadoEm,
        Comentarios = chamado.Comentarios
            .OrderBy(c => c.CriadoEm)
            .Select(c => c.ToDto())
            .ToList(),
        Historico = chamado.Historico
            .OrderByDescending(h => h.CriadoEm)
            .Select(h => h.ToDto())
            .ToList(),
        Resolucoes = chamado.Resolucoes
            .OrderBy(r => r.CriadoEm)
            .Select(r => r.ToDto())
            .ToList()
    };

    public static ComentarioChamadoDto ToDto(this ComentarioChamado comentario) => new()
    {
        Id = comentario.Id,
        UsuarioId = comentario.UsuarioId,
        UsuarioNome = comentario.Usuario?.Nome ?? string.Empty,
        Mensagem = comentario.Mensagem,
        CriadoEm = comentario.CriadoEm,
        Tipo = comentario.Tipo,
        Responsavel = comentario.Responsavel,
        Situacao = comentario.Situacao,
        Resposta = comentario.Resposta,
        RespondidoPorNome = comentario.RespondidoPor?.Nome,
        RespondidoEm = comentario.RespondidoEm
    };

    public static ResolucaoChamadoDto ToDto(this ResolucaoChamado resolucao) => new()
    {
        Id = resolucao.Id,
        UsuarioId = resolucao.UsuarioId,
        UsuarioNome = resolucao.Usuario?.Nome ?? string.Empty,
        Descricao = resolucao.Descricao,
        Situacao = resolucao.Situacao,
        MotivoRecusa = resolucao.MotivoRecusa,
        CriadoEm = resolucao.CriadoEm,
        AvaliadoPorNome = resolucao.AvaliadoPor?.Nome,
        AvaliadoEm = resolucao.AvaliadoEm
    };
}
