import { useEffect, useMemo, useState } from "react";
import type { FormEvent } from "react";
import { useParams } from "react-router-dom";
import {
  addChamadoComentario,
  aprovarResolucao,
  getChamadoById,
  proporResolucao,
  updateChamado,
} from "../api/chamadosApi";
import { getTecnicos } from "../api/usuariosApi";
import { getCategorias } from "../api/categoriasApi";
import { getApiErrorMessage } from "../api/apiClient";
import { ALL_PRIORITIES, ALL_STATUSES, PriorityBadge, STATUS_LABEL_MAP, StatusBadge } from "../components/Badges";
import { RichTextEditor } from "../components/RichTextEditor";
import { RichTextView } from "../components/RichTextView";
import { useAuth } from "../context/AuthContext";
import type {
  AcaoHistorico,
  Categoria,
  ChamadoDetalhe,
  PrioridadeChamado,
  StatusChamado,
  Usuario,
} from "../types";

type Tab = "detalhes" | "comentarios" | "historico";

const ACAO_LABELS: Record<AcaoHistorico, string> = {
  status: "Status alterado",
  prioridade: "Prioridade alterada",
  categoria: "Categoria alterada",
  tecnico: "Técnico alterado",
  comentario: "Comentário adicionado",
  resolucao_proposta: "Resolução proposta",
  resolucao_aprovada: "Resolução aprovada",
};

interface ManagementForm {
  status: StatusChamado;
  prioridade: PrioridadeChamado;
  categoriaId: string;
  tecnicoId: string;
}

export function ChamadoDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { user } = useAuth();

  const [chamado, setChamado] = useState<ChamadoDetalhe | null>(null);
  const [tecnicos, setTecnicos] = useState<Usuario[]>([]);
  const [categorias, setCategorias] = useState<Categoria[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [tab, setTab] = useState<Tab>("detalhes");

  const [mensagem, setMensagem] = useState("");
  const [isSendingComment, setIsSendingComment] = useState(false);

  const [form, setForm] = useState<ManagementForm | null>(null);
  const [isSavingForm, setIsSavingForm] = useState(false);

  const [resolucaoHtml, setResolucaoHtml] = useState("");
  const [isSendingResolucao, setIsSendingResolucao] = useState(false);
  const [isApproving, setIsApproving] = useState(false);
  const [isEditingResolucao, setIsEditingResolucao] = useState(false);

  const canManage = user?.perfil === "Administrador" || user?.perfil === "Tecnico";
  const isOwner = user?.id === chamado?.solicitanteId;
  const canApprove = chamado ? (isOwner || user?.perfil === "Administrador") : false;

  const load = async () => {
    if (!id) return;
    setIsLoading(true);
    setError(null);
    try {
      const data = await getChamadoById(id);
      setChamado(data);
      setForm({
        status: data.status,
        prioridade: data.prioridade,
        categoriaId: data.categoriaId ?? "",
        tecnicoId: data.tecnicoId ?? "",
      });
      setResolucaoHtml(data.resolucao ?? "");
      setIsEditingResolucao(false);
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível carregar o chamado."));
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, [id]);

  useEffect(() => {
    if (canManage) {
      getTecnicos().then(setTecnicos).catch(() => setTecnicos([]));
      getCategorias().then(setCategorias).catch(() => setCategorias([]));
    }
  }, [canManage]);

  const availableCategorias = useMemo(() => {
    if (!chamado) return categorias;
    const activeOrCurrent = categorias.filter((c) => c.ativa || c.id === chamado.categoriaId);
    if (chamado.categoriaId && !activeOrCurrent.some((c) => c.id === chamado.categoriaId) && chamado.categoriaNome) {
      return [
        ...activeOrCurrent,
        { id: chamado.categoriaId, nome: chamado.categoriaNome, descricao: null, ativa: false, criadoEm: "" },
      ];
    }
    return activeOrCurrent;
  }, [categorias, chamado]);

  const formDirty = useMemo(() => {
    if (!chamado || !form) return false;
    return (
      form.status !== chamado.status ||
      form.prioridade !== chamado.prioridade ||
      form.categoriaId !== (chamado.categoriaId ?? "") ||
      form.tecnicoId !== (chamado.tecnicoId ?? "")
    );
  }, [chamado, form]);

  const handleSaveManagement = async () => {
    if (!id || !chamado || !form) return;
    setError(null);
    setIsSavingForm(true);
    try {
      const payload: Partial<{
        status: StatusChamado;
        prioridade: PrioridadeChamado;
        categoriaId: string | null;
        tecnicoId: string | null;
      }> = {};
      if (form.status !== chamado.status) payload.status = form.status;
      if (form.prioridade !== chamado.prioridade) payload.prioridade = form.prioridade;
      if (form.categoriaId !== (chamado.categoriaId ?? "")) payload.categoriaId = form.categoriaId || null;
      if (form.tecnicoId !== (chamado.tecnicoId ?? "")) payload.tecnicoId = form.tecnicoId || null;

      await updateChamado(id, payload);
      await load();
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível atualizar o chamado."));
    } finally {
      setIsSavingForm(false);
    }
  };

  const handleAddComment = async (event: FormEvent) => {
    event.preventDefault();
    if (!id || !mensagem.trim() || mensagem === "<p></p>") return;

    setIsSendingComment(true);
    setError(null);
    try {
      await addChamadoComentario(id, mensagem);
      setMensagem("");
      await load();
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível enviar o comentário."));
    } finally {
      setIsSendingComment(false);
    }
  };

  const handleProporResolucao = async () => {
    if (!id) return;
    setError(null);
    setIsSendingResolucao(true);
    try {
      await proporResolucao(id, resolucaoHtml);
      await load();
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível enviar a resolução."));
    } finally {
      setIsSendingResolucao(false);
    }
  };

  const handleAprovarResolucao = async () => {
    if (!id) return;
    setError(null);
    setIsApproving(true);
    try {
      await aprovarResolucao(id);
      await load();
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível aprovar a resolução."));
    } finally {
      setIsApproving(false);
    }
  };

  if (isLoading) return <p className="text-surface-500 dark:text-surface-400">Carregando...</p>;
  if (error && !chamado) return <div className="alert-error">{error}</div>;
  if (!chamado || !form) return null;

  const faltaCategoriaOuTecnico = !chamado.categoriaId || !chamado.tecnicoId;
  const mostrarEditorResolucao = canManage && (!chamado.resolucao || isEditingResolucao);
  const podeAlterarResolucao = canManage && !!chamado.resolucao && !chamado.resolucaoAprovada && !isEditingResolucao;

  const historicoOrdenado = [...chamado.historico].sort(
    (a, b) => new Date(b.criadoEm).getTime() - new Date(a.criadoEm).getTime()
  );

  return (
    <div className="flex flex-col gap-6">
      <div className="flex flex-wrap items-center justify-between gap-4">
        <h1 className="text-2xl font-bold text-surface-900 dark:text-white">{chamado.titulo}</h1>
        <div className="flex gap-2">
          <StatusBadge status={chamado.status} />
          <PriorityBadge priority={chamado.prioridade} />
        </div>
      </div>

      {error && <div className="alert-error">{error}</div>}

      <div className="flex flex-wrap gap-2">
        <button
          type="button"
          className={tab === "detalhes" ? "tab-btn-active" : "tab-btn-inactive"}
          onClick={() => setTab("detalhes")}
        >
          Detalhes
        </button>
        <button
          type="button"
          className={tab === "comentarios" ? "tab-btn-active" : "tab-btn-inactive"}
          onClick={() => setTab("comentarios")}
        >
          Comentários ({chamado.comentarios.length})
        </button>
        <button
          type="button"
          className={tab === "historico" ? "tab-btn-active" : "tab-btn-inactive"}
          onClick={() => setTab("historico")}
        >
          Histórico ({chamado.historico.length})
        </button>
      </div>

      {tab === "detalhes" && (
        <div className="flex flex-col gap-6">
          <div className="card">
            <RichTextView html={chamado.descricao} />
            <dl className="mt-4 grid grid-cols-[max-content_1fr] gap-x-6 gap-y-2 text-sm">
              <dt className="text-surface-500 dark:text-surface-400">Solicitante</dt>
              <dd className="text-surface-900 dark:text-surface-100">{chamado.solicitanteNome}</dd>
              <dt className="text-surface-500 dark:text-surface-400">Técnico</dt>
              <dd className="text-surface-900 dark:text-surface-100">{chamado.tecnicoNome ?? "Não atribuído"}</dd>
              <dt className="text-surface-500 dark:text-surface-400">Categoria</dt>
              <dd className="text-surface-900 dark:text-surface-100">{chamado.categoriaNome ?? "Sem categoria"}</dd>
              <dt className="text-surface-500 dark:text-surface-400">Criado em</dt>
              <dd className="text-surface-900 dark:text-surface-100">
                {new Date(chamado.criadoEm).toLocaleString("pt-BR")}
              </dd>
              {chamado.fechadoEm && (
                <>
                  <dt className="text-surface-500 dark:text-surface-400">Fechado em</dt>
                  <dd className="text-surface-900 dark:text-surface-100">
                    {new Date(chamado.fechadoEm).toLocaleString("pt-BR")}
                  </dd>
                </>
              )}
            </dl>
          </div>

          {canManage && (
            <div className="card flex flex-col gap-4">
              <h2 className="text-lg font-semibold text-surface-900 dark:text-white">Gerenciar chamado</h2>
              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <label className="flex flex-col gap-1.5">
                  <span className="field-label">Status</span>
                  <select
                    className="field-input"
                    value={form.status}
                    onChange={(e) => setForm({ ...form, status: e.target.value as StatusChamado })}
                  >
                    {ALL_STATUSES.map((s) => (
                      <option key={s} value={s}>
                        {STATUS_LABEL_MAP[s]}
                      </option>
                    ))}
                  </select>
                </label>

                <label className="flex flex-col gap-1.5">
                  <span className="field-label">Prioridade</span>
                  <select
                    className="field-input"
                    value={form.prioridade}
                    onChange={(e) => setForm({ ...form, prioridade: e.target.value as PrioridadeChamado })}
                  >
                    {ALL_PRIORITIES.map((p) => (
                      <option key={p} value={p}>
                        {p}
                      </option>
                    ))}
                  </select>
                </label>

                <label className="flex flex-col gap-1.5">
                  <span className="field-label">Categoria</span>
                  <select
                    className="field-input"
                    value={form.categoriaId}
                    onChange={(e) => setForm({ ...form, categoriaId: e.target.value })}
                  >
                    <option value="">Sem categoria</option>
                    {availableCategorias.map((categoria) => (
                      <option key={categoria.id} value={categoria.id}>
                        {categoria.nome}
                      </option>
                    ))}
                  </select>
                </label>

                <label className="flex flex-col gap-1.5">
                  <span className="field-label">Técnico</span>
                  <select
                    className="field-input"
                    value={form.tecnicoId}
                    onChange={(e) => setForm({ ...form, tecnicoId: e.target.value })}
                  >
                    <option value="">Não atribuído</option>
                    {tecnicos.map((tecnico) => (
                      <option key={tecnico.id} value={tecnico.id}>
                        {tecnico.nome}
                      </option>
                    ))}
                  </select>
                </label>
              </div>

              <button
                type="button"
                className="btn-primary self-start"
                disabled={!formDirty || isSavingForm}
                onClick={handleSaveManagement}
              >
                {isSavingForm ? "Salvando..." : "Salvar alterações"}
              </button>
            </div>
          )}

          <div className="card flex flex-col gap-4">
            <h2 className="text-lg font-semibold text-surface-900 dark:text-white">Resolução</h2>

            {chamado.resolucao && chamado.resolucaoAprovada && (
              <div className="flex flex-col gap-3">
                <div className="alert-success">
                  Resolução aprovada
                  {chamado.concluidoEm && ` em ${new Date(chamado.concluidoEm).toLocaleString("pt-BR")}`}.
                </div>
                <RichTextView html={chamado.resolucao} />
              </div>
            )}

            {chamado.resolucao && !chamado.resolucaoAprovada && !isEditingResolucao && (
              <div className="flex flex-col gap-3">
                <div className="alert-warning">Resolução pendente de aprovação do solicitante.</div>
                <RichTextView html={chamado.resolucao} />
                <div className="flex flex-wrap gap-2">
                  {canApprove && (
                    <button
                      type="button"
                      className="btn-primary self-start"
                      disabled={isApproving}
                      onClick={handleAprovarResolucao}
                    >
                      {isApproving ? "Aprovando..." : "Aprovar resolução"}
                    </button>
                  )}
                  {podeAlterarResolucao && (
                    <button
                      type="button"
                      className="btn-secondary self-start"
                      onClick={() => setIsEditingResolucao(true)}
                    >
                      Alterar resolução
                    </button>
                  )}
                </div>
              </div>
            )}

            {mostrarEditorResolucao && (
              <div className="flex flex-col gap-3">
                {faltaCategoriaOuTecnico && (
                  <div className="alert-warning">
                    Defina categoria e técnico responsável antes de propor uma resolução.
                  </div>
                )}
                <RichTextEditor
                  value={resolucaoHtml}
                  onChange={setResolucaoHtml}
                  placeholder="Descreva a resolução aplicada..."
                  readOnly={faltaCategoriaOuTecnico}
                />
                <div className="flex flex-wrap gap-2">
                  <button
                    type="button"
                    className="btn-primary self-start"
                    disabled={faltaCategoriaOuTecnico || isSendingResolucao}
                    onClick={handleProporResolucao}
                  >
                    {isSendingResolucao ? "Enviando..." : "Enviar resolução"}
                  </button>
                  {isEditingResolucao && (
                    <button
                      type="button"
                      className="btn-ghost self-start"
                      onClick={() => {
                        setResolucaoHtml(chamado.resolucao ?? "");
                        setIsEditingResolucao(false);
                      }}
                    >
                      Cancelar
                    </button>
                  )}
                </div>
              </div>
            )}

            {!canManage && !chamado.resolucao && (
              <p className="text-sm text-surface-500 dark:text-surface-400">
                Ainda não há uma resolução proposta para este chamado.
              </p>
            )}
          </div>
        </div>
      )}

      {tab === "comentarios" && (
        <div className="card flex flex-col gap-4">
          <ul className="flex flex-col gap-3">
            {chamado.comentarios.length === 0 && (
              <li className="text-sm text-surface-500 dark:text-surface-400">Nenhum comentário ainda.</li>
            )}
            {chamado.comentarios.map((comentario) => (
              <li
                key={comentario.id}
                className="rounded-lg border border-surface-200 p-3 dark:border-surface-800"
              >
                <div className="mb-1 flex items-center justify-between text-sm">
                  <strong className="text-surface-900 dark:text-surface-100">{comentario.usuarioNome}</strong>
                  <span className="text-surface-500 dark:text-surface-400">
                    {new Date(comentario.criadoEm).toLocaleString("pt-BR")}
                  </span>
                </div>
                <RichTextView html={comentario.mensagem} />
              </li>
            ))}
          </ul>

          <form className="flex flex-col gap-3" onSubmit={handleAddComment}>
            <div className="flex flex-col gap-1.5">
              <span className="field-label">Novo comentário</span>
              <RichTextEditor value={mensagem} onChange={setMensagem} placeholder="Escreva um comentário..." />
            </div>
            <button type="submit" className="btn-primary self-start" disabled={isSendingComment}>
              {isSendingComment ? "Enviando..." : "Comentar"}
            </button>
          </form>
        </div>
      )}

      {tab === "historico" && (
        <div className="card">
          <ul className="flex flex-col gap-3">
            {historicoOrdenado.length === 0 && (
              <li className="text-sm text-surface-500 dark:text-surface-400">Nenhum evento registrado ainda.</li>
            )}
            {historicoOrdenado.map((evento) => (
              <li
                key={evento.id}
                className="flex flex-col gap-1 rounded-lg border border-surface-200 p-3 text-sm dark:border-surface-800"
              >
                <div className="flex items-center justify-between">
                  <strong className="text-surface-900 dark:text-surface-100">{ACAO_LABELS[evento.acao]}</strong>
                  <span className="text-surface-500 dark:text-surface-400">
                    {new Date(evento.criadoEm).toLocaleString("pt-BR")}
                  </span>
                </div>
                <span className="text-surface-600 dark:text-surface-300">
                  {evento.valorAnterior && evento.valorNovo
                    ? `de ${evento.valorAnterior} para ${evento.valorNovo}`
                    : evento.valorNovo ?? ""}
                </span>
                <span className="text-xs text-surface-500 dark:text-surface-400">por {evento.usuarioNome}</span>
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}
