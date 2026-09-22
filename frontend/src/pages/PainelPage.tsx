import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import { Link } from "react-router-dom";
import { createChamado, getPainelChamados } from "../api/chamadosApi";
import { getCategorias } from "../api/categoriasApi";
import { getApiErrorMessage } from "../api/apiClient";
import { ALL_PRIORITIES, PriorityBadge, StatusBadge } from "../components/Badges";
import { RichTextEditor } from "../components/RichTextEditor";
import type { Categoria, Chamado, Pagina, PainelChamados, PrioridadeChamado } from "../types";

const PAGE_SIZE = 10;

type SectionKey = "abertos" | "atribuidos" | "concluidos";

const SECTION_LABELS: Record<SectionKey, string> = {
  abertos: "Chamados em aberto",
  atribuidos: "Chamados atribuídos",
  concluidos: "Chamados concluídos",
};

function ChamadosTable({ pagina, onPageChange }: { pagina: Pagina<Chamado>; onPageChange: (page: number) => void }) {
  return (
    <div className="flex flex-col gap-3">
      <div className="table-shell">
        <table className="data-table">
          <thead>
            <tr>
              <th>Título</th>
              <th>Status</th>
              <th>Prioridade</th>
              <th>Solicitante</th>
              <th>Técnico</th>
              <th>Criado em</th>
            </tr>
          </thead>
          <tbody>
            {pagina.itens.length === 0 && (
              <tr>
                <td colSpan={6} className="text-center text-surface-500 dark:text-surface-400">
                  Nenhum chamado nesta seção.
                </td>
              </tr>
            )}
            {pagina.itens.map((chamado) => (
              <tr key={chamado.id}>
                <td>
                  <Link
                    to={`/chamados/${chamado.id}`}
                    className="font-medium text-brand-700 hover:underline dark:text-brand-300"
                  >
                    {chamado.titulo}
                  </Link>
                </td>
                <td>
                  <StatusBadge status={chamado.status} />
                </td>
                <td>
                  <PriorityBadge priority={chamado.prioridade} />
                </td>
                <td className="text-surface-600 dark:text-surface-300">{chamado.solicitanteNome}</td>
                <td className="text-surface-600 dark:text-surface-300">{chamado.tecnicoNome ?? "—"}</td>
                <td className="text-surface-600 dark:text-surface-300">
                  {new Date(chamado.criadoEm).toLocaleString("pt-BR")}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {pagina.totalPaginas > 1 && (
        <div className="flex items-center justify-between text-sm text-surface-600 dark:text-surface-300">
          <button
            type="button"
            className="action-btn btn-secondary"
            disabled={pagina.paginaAtual <= 1}
            onClick={() => onPageChange(pagina.paginaAtual - 1)}
          >
            Anterior
          </button>
          <span>
            Página {pagina.paginaAtual} de {pagina.totalPaginas} ({pagina.totalItens} chamados)
          </span>
          <button
            type="button"
            className="action-btn btn-secondary"
            disabled={pagina.paginaAtual >= pagina.totalPaginas}
            onClick={() => onPageChange(pagina.paginaAtual + 1)}
          >
            Próxima
          </button>
        </div>
      )}
    </div>
  );
}

export function PainelPage() {
  const [painel, setPainel] = useState<PainelChamados | null>(null);
  const [categorias, setCategorias] = useState<Categoria[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [paginaAbertos, setPaginaAbertos] = useState(1);
  const [paginaAtribuidos, setPaginaAtribuidos] = useState(1);
  const [paginaConcluidos, setPaginaConcluidos] = useState(1);

  const [activeTab, setActiveTab] = useState<SectionKey>("abertos");

  const [showForm, setShowForm] = useState(false);
  const [titulo, setTitulo] = useState("");
  const [descricao, setDescricao] = useState("");
  const [prioridade, setPrioridade] = useState<PrioridadeChamado>("Media");
  const [categoriaId, setCategoriaId] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const loadPainel = async () => {
    setIsLoading(true);
    setError(null);
    try {
      const data = await getPainelChamados({
        tamanhoPagina: PAGE_SIZE,
        paginaAbertos,
        paginaAtribuidos,
        paginaConcluidos,
      });
      setPainel(data);
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível carregar o painel de chamados."));
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadPainel();
  }, [paginaAbertos, paginaAtribuidos, paginaConcluidos]);

  useEffect(() => {
    getCategorias().then(setCategorias).catch(() => setCategorias([]));
  }, []);

  const handleCreate = async (event: FormEvent) => {
    event.preventDefault();
    setIsSubmitting(true);
    setError(null);
    try {
      await createChamado({
        titulo,
        descricao,
        prioridade,
        categoriaId: categoriaId || undefined,
      });
      setTitulo("");
      setDescricao("");
      setPrioridade("Media");
      setCategoriaId("");
      setShowForm(false);
      await loadPainel();
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível abrir o chamado."));
    } finally {
      setIsSubmitting(false);
    }
  };

  const sections: SectionKey[] = ["abertos", "atribuidos", "concluidos"];

  return (
    <div className="flex flex-col gap-6">
      <div className="flex items-center justify-between gap-4">
        <h1 className="text-2xl font-bold text-surface-900 dark:text-white">Painel de chamados</h1>
        <button type="button" className="btn-primary" onClick={() => setShowForm((v) => !v)}>
          {showForm ? "Cancelar" : "Novo chamado"}
        </button>
      </div>

      {error && <div className="alert-error">{error}</div>}

      {showForm && (
        <form className="card flex flex-col gap-4" onSubmit={handleCreate}>
          <label className="flex flex-col gap-1.5">
            <span className="field-label">Título</span>
            <input
              className="field-input"
              value={titulo}
              onChange={(e) => setTitulo(e.target.value)}
              required
              maxLength={200}
            />
          </label>

          <div className="flex flex-col gap-1.5">
            <span className="field-label">Descrição</span>
            <RichTextEditor value={descricao} onChange={setDescricao} placeholder="Descreva o problema..." />
          </div>

          <div className="flex flex-col gap-4 sm:flex-row">
            <label className="flex flex-1 flex-col gap-1.5">
              <span className="field-label">Prioridade</span>
              <select
                className="field-input"
                value={prioridade}
                onChange={(e) => setPrioridade(e.target.value as PrioridadeChamado)}
              >
                {ALL_PRIORITIES.map((p) => (
                  <option key={p} value={p}>
                    {p}
                  </option>
                ))}
              </select>
            </label>

            <label className="flex flex-1 flex-col gap-1.5">
              <span className="field-label">Categoria (opcional)</span>
              <select className="field-input" value={categoriaId} onChange={(e) => setCategoriaId(e.target.value)}>
                <option value="">Sem categoria</option>
                {categorias.map((categoria) => (
                  <option key={categoria.id} value={categoria.id}>
                    {categoria.nome}
                  </option>
                ))}
              </select>
            </label>
          </div>

          <button type="submit" className="btn-primary self-start" disabled={isSubmitting}>
            {isSubmitting ? "Enviando..." : "Abrir chamado"}
          </button>
        </form>
      )}

      <div className="flex flex-wrap gap-2">
        {sections.map((section) => (
          <button
            key={section}
            type="button"
            className={activeTab === section ? "tab-btn-active" : "tab-btn-inactive"}
            onClick={() => setActiveTab(section)}
          >
            {SECTION_LABELS[section]}
            {painel ? ` (${painel[section].totalItens})` : ""}
          </button>
        ))}
      </div>

      {isLoading && !painel ? (
        <p className="text-surface-500 dark:text-surface-400">Carregando painel...</p>
      ) : painel ? (
        <div className="card">
          <h2 className="mb-4 text-lg font-semibold text-surface-900 dark:text-white">
            {SECTION_LABELS[activeTab]}
          </h2>
          {activeTab === "abertos" && (
            <ChamadosTable pagina={painel.abertos} onPageChange={setPaginaAbertos} />
          )}
          {activeTab === "atribuidos" && (
            <ChamadosTable pagina={painel.atribuidos} onPageChange={setPaginaAtribuidos} />
          )}
          {activeTab === "concluidos" && (
            <ChamadosTable pagina={painel.concluidos} onPageChange={setPaginaConcluidos} />
          )}
        </div>
      ) : null}
    </div>
  );
}
