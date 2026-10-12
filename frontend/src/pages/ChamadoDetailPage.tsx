import { useEffect, useMemo, useState } from "react";
import type { FormEvent } from "react";
import { useParams } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faFloppyDisk, faPlus } from "@fortawesome/free-solid-svg-icons";
import {
  addChamadoComentario,
  aprovarResolucao,
  getChamadoById,
  proporResolucao,
  recusarResolucao,
  updateChamado,
} from "../api/chamadosApi";
import { getTecnicos } from "../api/usuariosApi";
import { getCategorias } from "../api/categoriasApi";
import { getModelosTexto } from "../api/modelosTextoApi";
import { getApiErrorMessage } from "../api/apiClient";
import { ALL_PRIORITIES, ALL_STATUSES, PriorityBadge, STATUS_LABEL_MAP, StatusBadge } from "../components/Badges";
import { RichTextEditor } from "../components/RichTextEditor";
import { RichTextView } from "../components/RichTextView";
import { useAuth } from "../context/AuthContext";
import type {
  AcaoHistorico,
  Categoria,
  ChamadoDetalhe,
  ComentarioChamado,
  ModeloTexto,
  PrioridadeChamado,
  ResolucaoChamado,
  SituacaoResolucao,
  StatusChamado,
  Usuario,
} from "../types";

type Tab = "detalhes" | "historico";
type TipoRegistro = "comentario" | "resolucao";

type ItemTimeline =
  | { tipo: "comentario"; criadoEm: string; comentario: ComentarioChamado }
  | { tipo: "resolucao"; criadoEm: string; resolucao: ResolucaoChamado };

const ACAO_LABELS: Record<AcaoHistorico, string> = {
  status: "Status alterado",
  prioridade: "Prioridade alterada",
  categoria: "Categoria alterada",
  tecnico: "Técnico alterado",
  comentario: "Comentário adicionado",
  resolucao_proposta: "Resolução proposta",
  resolucao_aprovada: "Resolução aprovada",
  resolucao_recusada: "Resolução recusada",
};

const SITUACAO_RESOLUCAO_LABELS: Record<SituacaoResolucao, string> = {
  Pendente: "Aguardando aprovação",
  Aprovada: "Aprovada",
  Recusada: "Recusada",
};

const SITUACAO_RESOLUCAO_CLASSES: Record<SituacaoResolucao, string> = {
  Pendente: "bg-amber-100 text-amber-800 dark:bg-amber-950/60 dark:text-amber-300",
  Aprovada: "bg-emerald-100 text-emerald-700 dark:bg-emerald-950/60 dark:text-emerald-300",
  Recusada: "bg-red-100 text-red-700 dark:bg-red-950/60 dark:text-red-300",
};

const SITUACAO_RESOLUCAO_DOT: Record<SituacaoResolucao, string> = {
  Pendente: "bg-amber-500",
  Aprovada: "bg-emerald-500",
  Recusada: "bg-red-500",
};

interface ManagementForm {
  status: StatusChamado;
  prioridade: PrioridadeChamado;
  categoriaId: string;
  tecnicoId: string;
}

function isHtmlVazio(html: string) {
  return html.replace(/<p><\/p>/g, "").replace(/<[^>]+>/g, "").trim() === "" && !html.includes("<img");
}

function formatarData(data: string) {
  return new Date(data).toLocaleString("pt-BR");
}

export function ChamadoDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { user } = useAuth();

  const [chamado, setChamado] = useState<ChamadoDetalhe | null>(null);
  const [tecnicos, setTecnicos] = useState<Usuario[]>([]);
  const [categorias, setCategorias] = useState<Categoria[]>([]);
  const [modelos, setModelos] = useState<ModeloTexto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [tab, setTab] = useState<Tab>("detalhes");

  const [form, setForm] = useState<ManagementForm | null>(null);
  const [isSavingForm, setIsSavingForm] = useState(false);

  const [isComposerOpen, setIsComposerOpen] = useState(false);
  const [tipoRegistro, setTipoRegistro] = useState<TipoRegistro>("comentario");
  const [texto, setTexto] = useState("");
  const [modeloSelecionado, setModeloSelecionado] = useState("");
  const [isSending, setIsSending] = useState(false);

  const [isApproving, setIsApproving] = useState(false);
  const [recusandoId, setRecusandoId] = useState<string | null>(null);
  const [motivoRecusa, setMotivoRecusa] = useState("");
  const [isRefusing, setIsRefusing] = useState(false);

  const canManage = user?.perfil === "Administrador" || user?.perfil === "Tecnico";
  const isOwner = user?.id === chamado?.solicitanteId;
  const canApprove = chamado ? isOwner || user?.perfil === "Administrador" : false;

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

  useEffect(() => {
    getModelosTexto()
      .then((data) => setModelos(data.filter((m) => m.ativo)))
      .catch(() => setModelos([]));
  }, []);

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

  const timeline = useMemo<ItemTimeline[]>(() => {
    if (!chamado) return [];
    const itens: ItemTimeline[] = [
      ...chamado.comentarios.map((comentario) => ({
        tipo: "comentario" as const,
        criadoEm: comentario.criadoEm,
        comentario,
      })),
      ...chamado.resolucoes.map((resolucao) => ({
        tipo: "resolucao" as const,
        criadoEm: resolucao.criadoEm,
        resolucao,
      })),
    ];
    return itens.sort((a, b) => new Date(a.criadoEm).getTime() - new Date(b.criadoEm).getTime());
  }, [chamado]);

  const historicoOrdenado = useMemo(() => {
    if (!chamado) return [];
    return [...chamado.historico].sort((a, b) => new Date(b.criadoEm).getTime() - new Date(a.criadoEm).getTime());
  }, [chamado]);

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

  const fecharComposer = () => {
    setIsComposerOpen(false);
    setTipoRegistro("comentario");
    setTexto("");
    setModeloSelecionado("");
  };

  const handleAplicarModelo = (modeloId: string) => {
    setModeloSelecionado(modeloId);
    const modelo = modelos.find((m) => m.id === modeloId);
    if (!modelo) return;
    setTexto((atual) => (isHtmlVazio(atual) ? modelo.conteudo : `${atual}${modelo.conteudo}`));
  };

  const handleEnviarRegistro = async (event: FormEvent) => {
    event.preventDefault();
    if (!id || isHtmlVazio(texto)) return;

    setIsSending(true);
    setError(null);
    try {
      if (tipoRegistro === "resolucao") {
        await proporResolucao(id, texto);
      } else {
        await addChamadoComentario(id, texto);
      }
      fecharComposer();
      await load();
    } catch (err) {
      setError(
        getApiErrorMessage(
          err,
          tipoRegistro === "resolucao" ? "Não foi possível enviar a resolução." : "Não foi possível enviar o comentário."
        )
      );
    } finally {
      setIsSending(false);
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

  const handleRecusarResolucao = async () => {
    if (!id) return;
    setError(null);
    setIsRefusing(true);
    try {
      await recusarResolucao(id, motivoRecusa.trim() || undefined);
      setRecusandoId(null);
      setMotivoRecusa("");
      await load();
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível recusar a resolução."));
    } finally {
      setIsRefusing(false);
    }
  };

  if (isLoading && !chamado) return <p className="text-surface-500 dark:text-surface-400">Carregando...</p>;
  if (error && !chamado) return <div className="alert-error">{error}</div>;
  if (!chamado || !form) return null;

  const faltaCategoriaOuTecnico = !chamado.categoriaId || !chamado.tecnicoId;
  const possuiResolucaoPendente = chamado.resolucoes.some((r) => r.situacao === "Pendente");
  const isTecnicoResponsavel = user?.perfil === "Administrador" || user?.id === chamado.tecnicoId;
  const chamadoFechado = chamado.status === "Fechado";
  const podeProporResolucao = canManage && isTecnicoResponsavel && !possuiResolucaoPendente && !chamadoFechado;

  const renderComentario = (comentario: ComentarioChamado) => (
    <li key={`c-${comentario.id}`} className="relative pl-8">
      <span className="absolute left-0 top-5 h-3 w-3 rounded-full bg-brand-500 ring-4 ring-surface-100 dark:ring-surface-950" />
      <div className="card p-4">
        <div className="mb-2 flex flex-wrap items-center justify-between gap-2 text-sm">
          <div className="flex items-center gap-2">
            <strong className="text-surface-900 dark:text-surface-100">{comentario.usuarioNome}</strong>
            <span className="text-surface-500 dark:text-surface-400">comentou</span>
          </div>
          <span className="text-surface-500 dark:text-surface-400">{formatarData(comentario.criadoEm)}</span>
        </div>
        <RichTextView html={comentario.mensagem} />
      </div>
    </li>
  );

  const renderResolucao = (resolucao: ResolucaoChamado) => (
    <li key={`r-${resolucao.id}`} className="relative pl-8">
      <span
        className={`absolute left-0 top-5 h-3 w-3 rounded-full ring-4 ring-surface-100 dark:ring-surface-950 ${
          SITUACAO_RESOLUCAO_DOT[resolucao.situacao]
        }`}
      />
      <div className="card flex flex-col gap-3 p-4">
        <div className="flex flex-wrap items-center justify-between gap-2 text-sm">
          <div className="flex flex-wrap items-center gap-2">
            <strong className="text-surface-900 dark:text-surface-100">{resolucao.usuarioNome}</strong>
            <span className="text-surface-500 dark:text-surface-400">registrou uma resolução</span>
            <span
              className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-semibold ${
                SITUACAO_RESOLUCAO_CLASSES[resolucao.situacao]
              }`}
            >
              {SITUACAO_RESOLUCAO_LABELS[resolucao.situacao]}
            </span>
          </div>
          <span className="text-surface-500 dark:text-surface-400">{formatarData(resolucao.criadoEm)}</span>
        </div>

        <RichTextView html={resolucao.descricao} />

        {resolucao.situacao !== "Pendente" && resolucao.avaliadoEm && (
          <div className={resolucao.situacao === "Aprovada" ? "alert-success" : "alert-error"}>
            {resolucao.situacao === "Aprovada" ? "Aprovada" : "Recusada"}
            {resolucao.avaliadoPorNome && ` por ${resolucao.avaliadoPorNome}`} em {formatarData(resolucao.avaliadoEm)}.
            {resolucao.motivoRecusa && (
              <span className="mt-1 block">
                <strong>Motivo:</strong> {resolucao.motivoRecusa}
              </span>
            )}
          </div>
        )}

        {resolucao.situacao === "Pendente" && canApprove && recusandoId !== resolucao.id && (
          <div className="flex flex-wrap gap-2">
            <button
              type="button"
              className="btn-primary"
              disabled={isApproving}
              onClick={handleAprovarResolucao}
            >
              {isApproving ? "Aprovando..." : "Aprovar resolução"}
            </button>
            <button
              type="button"
              className="btn-secondary"
              onClick={() => {
                setRecusandoId(resolucao.id);
                setMotivoRecusa("");
              }}
            >
              Recusar
            </button>
          </div>
        )}

        {resolucao.situacao === "Pendente" && recusandoId === resolucao.id && (
          <div className="flex flex-col gap-2">
            <label className="flex flex-col gap-1.5">
              <span className="field-label">Motivo da recusa (opcional)</span>
              <textarea
                className="field-input"
                rows={3}
                value={motivoRecusa}
                onChange={(e) => setMotivoRecusa(e.target.value)}
                placeholder="Explique por que a resolução não resolveu o problema..."
              />
            </label>
            <div className="flex flex-wrap gap-2">
              <button type="button" className="btn-danger" disabled={isRefusing} onClick={handleRecusarResolucao}>
                {isRefusing ? "Recusando..." : "Confirmar recusa"}
              </button>
              <button type="button" className="btn-ghost" onClick={() => setRecusandoId(null)}>
                Cancelar
              </button>
            </div>
          </div>
        )}

        {resolucao.situacao === "Pendente" && !canApprove && (
          <p className="text-xs text-surface-500 dark:text-surface-400">Aguardando avaliação do solicitante.</p>
        )}
      </div>
    </li>
  );

  return (
    <div className={`grid grid-cols-1 gap-6 ${canManage ? "lg:grid-cols-[minmax(0,1fr)_320px]" : ""}`}>
      <div className="flex min-w-0 flex-col gap-6">
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
                <dd className="text-surface-900 dark:text-surface-100">{formatarData(chamado.criadoEm)}</dd>
                {chamado.fechadoEm && (
                  <>
                    <dt className="text-surface-500 dark:text-surface-400">Fechado em</dt>
                    <dd className="text-surface-900 dark:text-surface-100">{formatarData(chamado.fechadoEm)}</dd>
                  </>
                )}
              </dl>
            </div>

            <section className="flex flex-col gap-4">
              <h2 className="text-lg font-semibold text-surface-900 dark:text-white">Acompanhamento</h2>

              {timeline.length === 0 ? (
                <p className="text-sm text-surface-500 dark:text-surface-400">
                  Nenhum registro ainda. Use o botão Adicionar para registrar o primeiro acompanhamento.
                </p>
              ) : (
                <ol className="relative flex flex-col gap-4 before:absolute before:bottom-0 before:left-[5px] before:top-0 before:w-0.5 before:bg-surface-200 dark:before:bg-surface-800">
                  {timeline.map((item) =>
                    item.tipo === "comentario" ? renderComentario(item.comentario) : renderResolucao(item.resolucao)
                  )}
                </ol>
              )}

              {!isComposerOpen && (
                <button type="button" className="btn-primary self-start" onClick={() => setIsComposerOpen(true)}>
                  <FontAwesomeIcon icon={faPlus} />
                  Adicionar
                </button>
              )}

              {isComposerOpen && (
                <form className="card flex flex-col gap-4" onSubmit={handleEnviarRegistro}>
                  <div className="flex flex-wrap items-end gap-4">
                    {canManage && (
                      <label className="flex flex-col gap-1.5">
                        <span className="field-label">Tipo de registro</span>
                        <select
                          className="field-input"
                          value={tipoRegistro}
                          onChange={(e) => setTipoRegistro(e.target.value as TipoRegistro)}
                        >
                          <option value="comentario">Comentário</option>
                          <option value="resolucao" disabled={!podeProporResolucao}>
                            Resolução
                          </option>
                        </select>
                      </label>
                    )}

                    <label className="flex min-w-[200px] flex-1 flex-col gap-1.5">
                      <span className="field-label">Layout</span>
                      <select
                        className="field-input"
                        value={modeloSelecionado}
                        onChange={(e) => handleAplicarModelo(e.target.value)}
                        disabled={modelos.length === 0}
                      >
                        <option value="">
                          {modelos.length === 0 ? "Nenhum layout cadastrado" : "Selecione um layout..."}
                        </option>
                        {modelos.map((modelo) => (
                          <option key={modelo.id} value={modelo.id}>
                            {modelo.titulo}
                          </option>
                        ))}
                      </select>
                    </label>
                  </div>

                  {canManage && possuiResolucaoPendente && (
                    <p className="text-xs text-surface-500 dark:text-surface-400">
                      Já existe uma resolução aguardando avaliação. Uma nova resolução poderá ser registrada se ela
                      for recusada.
                    </p>
                  )}

                  {tipoRegistro === "resolucao" && faltaCategoriaOuTecnico && (
                    <div className="alert-warning">
                      Defina categoria e técnico responsável antes de registrar uma resolução.
                    </div>
                  )}

                  <RichTextEditor
                    value={texto}
                    onChange={setTexto}
                    placeholder={
                      tipoRegistro === "resolucao"
                        ? "Descreva a resolução aplicada..."
                        : "Escreva o acompanhamento do chamado..."
                    }
                  />

                  <div className="flex flex-wrap gap-2">
                    <button
                      type="submit"
                      className="btn-primary"
                      disabled={
                        isSending || isHtmlVazio(texto) || (tipoRegistro === "resolucao" && faltaCategoriaOuTecnico)
                      }
                    >
                      {isSending
                        ? "Enviando..."
                        : tipoRegistro === "resolucao"
                          ? "Registrar resolução"
                          : "Registrar comentário"}
                    </button>
                    <button type="button" className="btn-ghost" onClick={fecharComposer}>
                      Cancelar
                    </button>
                  </div>
                </form>
              )}
            </section>
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
                    <strong className="text-surface-900 dark:text-surface-100">
                      {ACAO_LABELS[evento.acao] ?? evento.acao}
                    </strong>
                    <span className="text-surface-500 dark:text-surface-400">{formatarData(evento.criadoEm)}</span>
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

      {canManage && (
        <aside className="lg:sticky lg:top-6 lg:self-start">
          <div className="card flex flex-col gap-4">
            <h2 className="text-lg font-semibold text-surface-900 dark:text-white">Gerenciar chamado</h2>

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

            <button
              type="button"
              className="btn-primary w-full"
              disabled={!formDirty || isSavingForm}
              onClick={handleSaveManagement}
            >
              <FontAwesomeIcon icon={faFloppyDisk} />
              {isSavingForm ? "Salvando..." : "Salvar"}
            </button>
          </div>
        </aside>
      )}
    </div>
  );
}
