using System.Text.RegularExpressions;
using HelpDesk.Api.Common.Exceptions;
using HelpDesk.Api.Common.Mappings;
using HelpDesk.Api.Data;
using HelpDesk.Api.DTOs.Chamados;
using HelpDesk.Api.DTOs.Comum;
using HelpDesk.Api.Entities;
using HelpDesk.Api.Entities.Enums;
using HelpDesk.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Api.Services;

public class ChamadoService : IChamadoService
{
    private const string ValorVazio = "—";

    private readonly AppDbContext _db;

    public ChamadoService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ChamadoDto>> GetForUserAsync(Guid userId, PerfilUsuario perfil, StatusChamado? status, bool? semAtribuicao, Guid? categoriaId)
    {
        var query = ChamadoQueryComIncludes();

        if (perfil == PerfilUsuario.Solicitante)
        {
            query = query.Where(c => c.SolicitanteId == userId);
        }

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        if (semAtribuicao.HasValue)
        {
            query = semAtribuicao.Value
                ? query.Where(c => c.TecnicoId == null)
                : query.Where(c => c.TecnicoId != null);
        }

        if (categoriaId.HasValue)
        {
            query = query.Where(c => c.CategoriaId == categoriaId.Value);
        }

        return await query
            .OrderByDescending(c => c.CriadoEm)
            .Select(c => c.ToDto())
            .ToListAsync();
    }

    public async Task<ChamadoDetalheDto> GetByIdAsync(Guid chamadoId, Guid userId, PerfilUsuario perfil)
    {
        var chamado = await FindChamadoOrThrowAsync(chamadoId, includeComentarios: true, includeHistorico: true);
        EnsureCanView(chamado, userId, perfil);
        return chamado.ToDetalheDto();
    }

    public async Task<ChamadoDto> CreateAsync(CreateChamadoRequest request, Guid solicitanteId)
    {
        var chamado = new Chamado
        {
            Id = Guid.NewGuid(),
            Titulo = request.Titulo.Trim(),
            Descricao = request.Descricao.Trim(),
            Prioridade = request.Prioridade,
            Status = StatusChamado.Aberto,
            CategoriaId = request.CategoriaId,
            SolicitanteId = solicitanteId,
            CriadoEm = DateTime.UtcNow
        };

        _db.Chamados.Add(chamado);
        await _db.SaveChangesAsync();

        await _db.Entry(chamado).Reference(c => c.Solicitante).LoadAsync();
        if (chamado.CategoriaId.HasValue)
        {
            await _db.Entry(chamado).Reference(c => c.Categoria).LoadAsync();
        }

        return chamado.ToDto();
    }

    public async Task<ChamadoDto> UpdateAsync(Guid chamadoId, UpdateChamadoRequest request, Guid userId, PerfilUsuario perfil)
    {
        if (perfil == PerfilUsuario.Solicitante)
        {
            throw new ForbiddenException("Solicitantes não podem alterar chamados.");
        }

        var chamado = await FindChamadoOrThrowAsync(chamadoId, includeComentarios: false, includeHistorico: false);
        var historico = new List<HistoricoChamado>();

        if (request.Status.HasValue && request.Status.Value != chamado.Status)
        {
            historico.Add(NovoHistorico(chamado.Id, userId, "status", chamado.Status.ToString(), request.Status.Value.ToString()));
            chamado.Status = request.Status.Value;
            chamado.FechadoEm = request.Status.Value == StatusChamado.Fechado ? DateTime.UtcNow : null;
        }

        if (request.Prioridade.HasValue && request.Prioridade.Value != chamado.Prioridade)
        {
            historico.Add(NovoHistorico(chamado.Id, userId, "prioridade", chamado.Prioridade.ToString(), request.Prioridade.Value.ToString()));
            chamado.Prioridade = request.Prioridade.Value;
        }

        if (request.CategoriaId.HasValue && request.CategoriaId.Value != chamado.CategoriaId)
        {
            var categoria = await _db.Categorias.FirstOrDefaultAsync(c => c.Id == request.CategoriaId.Value);
            if (categoria is null)
            {
                throw new BusinessRuleException("Categoria informada não existe.");
            }

            var nomeAnterior = chamado.Categoria?.Nome ?? ValorVazio;
            historico.Add(NovoHistorico(chamado.Id, userId, "categoria", nomeAnterior, categoria.Nome));
            chamado.CategoriaId = categoria.Id;
            chamado.Categoria = categoria;
        }

        if (request.TecnicoId.HasValue && request.TecnicoId.Value != chamado.TecnicoId)
        {
            var tecnico = await _db.Usuarios.FirstOrDefaultAsync(u =>
                u.Id == request.TecnicoId.Value &&
                u.Ativo &&
                (u.Perfil == PerfilUsuario.Tecnico || u.Perfil == PerfilUsuario.Administrador));

            if (tecnico is null)
            {
                throw new BusinessRuleException("Técnico inválido. Selecione um técnico ativo.");
            }

            var nomeAnterior = chamado.Tecnico?.Nome ?? ValorVazio;
            historico.Add(NovoHistorico(chamado.Id, userId, "tecnico", nomeAnterior, tecnico.Nome));
            chamado.TecnicoId = tecnico.Id;
            chamado.Tecnico = tecnico;
        }

        chamado.AtualizadoEm = DateTime.UtcNow;

        if (historico.Count > 0)
        {
            _db.Historico.AddRange(historico);
        }

        await _db.SaveChangesAsync();

        return chamado.ToDto();
    }

    public async Task<ComentarioChamadoDto> AddComentarioAsync(Guid chamadoId, CreateComentarioRequest request, Guid userId, PerfilUsuario perfil)
    {
        var chamado = await FindChamadoOrThrowAsync(chamadoId, includeComentarios: false, includeHistorico: false);
        EnsureCanView(chamado, userId, perfil);

        var mensagem = request.Mensagem.Trim();

        var comentario = new ComentarioChamado
        {
            Id = Guid.NewGuid(),
            ChamadoId = chamadoId,
            UsuarioId = userId,
            Mensagem = mensagem,
            CriadoEm = DateTime.UtcNow
        };

        _db.Comentarios.Add(comentario);

        _db.Historico.Add(NovoHistorico(chamado.Id, userId, "comentario", null, TruncarTexto(mensagem, 120)));

        chamado.AtualizadoEm = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        await _db.Entry(comentario).Reference(c => c.Usuario).LoadAsync();

        return comentario.ToDto();
    }

    public async Task<ChamadoDto> ProporResolucaoAsync(Guid chamadoId, ProporResolucaoRequest request, Guid userId, PerfilUsuario perfil)
    {
        if (perfil != PerfilUsuario.Tecnico && perfil != PerfilUsuario.Administrador)
        {
            throw new ForbiddenException("Apenas técnicos ou administradores podem propor uma resolução.");
        }

        var chamado = await FindChamadoOrThrowAsync(chamadoId, includeComentarios: false, includeHistorico: false);

        if (perfil == PerfilUsuario.Tecnico && chamado.TecnicoId != userId)
        {
            throw new ForbiddenException("Você não é o técnico responsável por este chamado.");
        }

        if (chamado.CategoriaId is null || chamado.TecnicoId is null)
        {
            throw new BusinessRuleException("Não é possível propor uma resolução sem categoria e técnico atribuídos ao chamado.");
        }

        var resolucao = request.Resolucao.Trim();

        chamado.Resolucao = resolucao;
        chamado.ResolucaoPropostaEm = DateTime.UtcNow;
        chamado.ResolucaoAprovada = false;
        chamado.Status = StatusChamado.Resolvido;
        chamado.AtualizadoEm = DateTime.UtcNow;

        _db.Historico.Add(NovoHistorico(chamado.Id, userId, "resolucao_proposta", null, TruncarTexto(resolucao, 120)));

        await _db.SaveChangesAsync();

        return chamado.ToDto();
    }

    public async Task<ChamadoDto> AprovarResolucaoAsync(Guid chamadoId, Guid userId, PerfilUsuario perfil)
    {
        var chamado = await FindChamadoOrThrowAsync(chamadoId, includeComentarios: false, includeHistorico: false);

        if (perfil != PerfilUsuario.Administrador && chamado.SolicitanteId != userId)
        {
            throw new ForbiddenException("Apenas o solicitante do chamado ou um administrador podem aprovar a resolução.");
        }

        if (chamado.Resolucao is null || chamado.ResolucaoAprovada)
        {
            throw new BusinessRuleException("Este chamado não possui uma resolução pendente de aprovação.");
        }

        chamado.ResolucaoAprovada = true;
        chamado.ConcluidoEm = DateTime.UtcNow;
        chamado.Status = StatusChamado.Fechado;
        chamado.FechadoEm = DateTime.UtcNow;
        chamado.AtualizadoEm = DateTime.UtcNow;

        _db.Historico.Add(NovoHistorico(chamado.Id, userId, "resolucao_aprovada", null, null));

        await _db.SaveChangesAsync();

        return chamado.ToDto();
    }

    public async Task<PainelChamadosDto> GetPainelAsync(Guid userId, PerfilUsuario perfil, int tamanhoPagina, int paginaAbertos, int paginaAtribuidos, int paginaConcluidos)
    {
        var abertosQuery = ChamadoQueryComIncludes()
            .Where(c => c.TecnicoId == null && c.Status == StatusChamado.Aberto);

        var atribuidosQuery = ChamadoQueryComIncludes()
            .Where(c => c.TecnicoId != null &&
                (c.Status == StatusChamado.EmAndamento || c.Status == StatusChamado.Aguardando || c.Status == StatusChamado.Resolvido));

        var concluidosQuery = ChamadoQueryComIncludes()
            .Where(c => c.Status == StatusChamado.Fechado);

        if (perfil == PerfilUsuario.Solicitante)
        {
            abertosQuery = abertosQuery.Where(c => c.SolicitanteId == userId);
            atribuidosQuery = atribuidosQuery.Where(c => c.SolicitanteId == userId);
            concluidosQuery = concluidosQuery.Where(c => c.SolicitanteId == userId);
        }

        abertosQuery = abertosQuery.OrderByDescending(c => c.CriadoEm);
        atribuidosQuery = atribuidosQuery.OrderByDescending(c => c.AtualizadoEm ?? c.CriadoEm);
        concluidosQuery = concluidosQuery.OrderByDescending(c => c.FechadoEm);

        return new PainelChamadosDto
        {
            Abertos = await PaginarAsync(abertosQuery, paginaAbertos, tamanhoPagina),
            Atribuidos = await PaginarAsync(atribuidosQuery, paginaAtribuidos, tamanhoPagina),
            Concluidos = await PaginarAsync(concluidosQuery, paginaConcluidos, tamanhoPagina)
        };
    }

    private IQueryable<Chamado> ChamadoQueryComIncludes()
    {
        return _db.Chamados
            .Include(c => c.Solicitante)
            .Include(c => c.Tecnico)
            .Include(c => c.Categoria)
            .AsQueryable();
    }

    private async Task<Chamado> FindChamadoOrThrowAsync(Guid chamadoId, bool includeComentarios, bool includeHistorico)
    {
        var query = ChamadoQueryComIncludes();

        if (includeComentarios)
        {
            query = query.Include(c => c.Comentarios).ThenInclude(cc => cc.Usuario);
        }

        if (includeHistorico)
        {
            query = query.Include(c => c.Historico).ThenInclude(h => h.Usuario);
        }

        var chamado = await query.FirstOrDefaultAsync(c => c.Id == chamadoId);
        if (chamado is null)
        {
            throw new NotFoundException("Chamado não encontrado.");
        }

        return chamado;
    }

    private static void EnsureCanView(Chamado chamado, Guid userId, PerfilUsuario perfil)
    {
        if (perfil == PerfilUsuario.Solicitante && chamado.SolicitanteId != userId)
        {
            throw new ForbiddenException("Você não tem permissão para acessar este chamado.");
        }
    }

    private static HistoricoChamado NovoHistorico(Guid chamadoId, Guid userId, string acao, string? valorAnterior, string? valorNovo)
    {
        return new HistoricoChamado
        {
            Id = Guid.NewGuid(),
            ChamadoId = chamadoId,
            UsuarioId = userId,
            Acao = acao,
            ValorAnterior = valorAnterior,
            ValorNovo = valorNovo,
            CriadoEm = DateTime.UtcNow
        };
    }

    private static string TruncarTexto(string texto, int tamanho)
    {
        var semTags = Regex.Replace(texto, "<[^>]+>", " ");
        semTags = Regex.Replace(semTags, "\\s+", " ").Trim();
        return semTags.Length <= tamanho ? semTags : semTags[..tamanho] + "...";
    }

    private static async Task<PaginaDto<ChamadoDto>> PaginarAsync(IQueryable<Chamado> query, int pagina, int tamanhoPagina)
    {
        var paginaValida = pagina < 1 ? 1 : pagina;
        var totalItens = await query.CountAsync();
        var itens = await query
            .Skip((paginaValida - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .Select(c => c.ToDto())
            .ToListAsync();

        var totalPaginas = totalItens == 0 ? 0 : (int)Math.Ceiling(totalItens / (double)tamanhoPagina);

        return new PaginaDto<ChamadoDto>
        {
            Itens = itens,
            PaginaAtual = paginaValida,
            TamanhoPagina = tamanhoPagina,
            TotalItens = totalItens,
            TotalPaginas = totalPaginas
        };
    }
}
