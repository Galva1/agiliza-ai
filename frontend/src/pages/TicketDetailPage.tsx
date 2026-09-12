import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import { useParams } from "react-router-dom";
import { addTicketComment, getTicketById, updateTicket } from "../api/ticketsApi";
import { getAssignableAgents } from "../api/usersApi";
import { getApiErrorMessage } from "../api/apiClient";
import { ALL_PRIORITIES, ALL_STATUSES, PriorityBadge, StatusBadge } from "../components/Badges";
import { useAuth } from "../context/AuthContext";
import type { TicketDetail, TicketPriority, TicketStatus, User } from "../types";

export function TicketDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { user } = useAuth();

  const [ticket, setTicket] = useState<TicketDetail | null>(null);
  const [agents, setAgents] = useState<User[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [message, setMessage] = useState("");
  const [isSendingComment, setIsSendingComment] = useState(false);

  const canManage = user?.role === "Admin" || user?.role === "Agente";

  const load = async () => {
    if (!id) return;
    setIsLoading(true);
    setError(null);
    try {
      const data = await getTicketById(id);
      setTicket(data);
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível carregar o chamado."));
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id]);

  useEffect(() => {
    if (canManage) {
      getAssignableAgents().then(setAgents).catch(() => setAgents([]));
    }
  }, [canManage]);

  const handleFieldChange = async (
    field: "status" | "priority" | "assigneeId",
    value: TicketStatus | TicketPriority | string
  ) => {
    if (!id) return;
    setError(null);
    try {
      const updated = await updateTicket(id, { [field]: value });
      setTicket((prev) => (prev ? { ...prev, ...updated } : prev));
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível atualizar o chamado."));
    }
  };

  const handleAddComment = async (event: FormEvent) => {
    event.preventDefault();
    if (!id || !message.trim()) return;

    setIsSendingComment(true);
    setError(null);
    try {
      const comment = await addTicketComment(id, message);
      setMessage("");
      setTicket((prev) => (prev ? { ...prev, comments: [...prev.comments, comment] } : prev));
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível enviar o comentário."));
    } finally {
      setIsSendingComment(false);
    }
  };

  if (isLoading) return <p className="page">Carregando...</p>;
  if (error && !ticket) return <p className="page alert alert-error">{error}</p>;
  if (!ticket) return null;

  return (
    <div className="page">
      <div className="page-header">
        <h1>{ticket.title}</h1>
        <div className="badge-row">
          <StatusBadge status={ticket.status} />
          <PriorityBadge priority={ticket.priority} />
        </div>
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      <div className="card">
        <p>{ticket.description}</p>
        <dl className="ticket-meta">
          <dt>Solicitante</dt>
          <dd>{ticket.requesterName}</dd>
          <dt>Responsável</dt>
          <dd>{ticket.assigneeName ?? "Não atribuído"}</dd>
          <dt>Criado em</dt>
          <dd>{new Date(ticket.createdAt).toLocaleString("pt-BR")}</dd>
          {ticket.closedAt && (
            <>
              <dt>Fechado em</dt>
              <dd>{new Date(ticket.closedAt).toLocaleString("pt-BR")}</dd>
            </>
          )}
        </dl>
      </div>

      {canManage && (
        <div className="card form">
          <h2>Gerenciar chamado</h2>
          <div className="form-row">
            <label className="field">
              <span>Status</span>
              <select
                value={ticket.status}
                onChange={(e) => handleFieldChange("status", e.target.value as TicketStatus)}
              >
                {ALL_STATUSES.map((s) => (
                  <option key={s} value={s}>
                    {s}
                  </option>
                ))}
              </select>
            </label>

            <label className="field">
              <span>Prioridade</span>
              <select
                value={ticket.priority}
                onChange={(e) => handleFieldChange("priority", e.target.value as TicketPriority)}
              >
                {ALL_PRIORITIES.map((p) => (
                  <option key={p} value={p}>
                    {p}
                  </option>
                ))}
              </select>
            </label>

            <label className="field">
              <span>Responsável</span>
              <select
                value={ticket.assigneeId ?? ""}
                onChange={(e) => handleFieldChange("assigneeId", e.target.value)}
              >
                <option value="" disabled>
                  Selecione um agente
                </option>
                {agents.map((agent) => (
                  <option key={agent.id} value={agent.id}>
                    {agent.name}
                  </option>
                ))}
              </select>
            </label>
          </div>
        </div>
      )}

      <div className="card">
        <h2>Comentários</h2>
        <ul className="comment-list">
          {ticket.comments.length === 0 && <li className="text-muted">Nenhum comentário ainda.</li>}
          {ticket.comments.map((comment) => (
            <li key={comment.id} className="comment-item">
              <div className="comment-header">
                <strong>{comment.userName}</strong>
                <span className="text-muted">
                  {new Date(comment.createdAt).toLocaleString("pt-BR")}
                </span>
              </div>
              <p>{comment.message}</p>
            </li>
          ))}
        </ul>

        <form className="form" onSubmit={handleAddComment}>
          <label className="field">
            <span>Novo comentário</span>
            <textarea
              value={message}
              onChange={(e) => setMessage(e.target.value)}
              rows={3}
              required
            />
          </label>
          <button type="submit" className="btn btn-primary" disabled={isSendingComment}>
            {isSendingComment ? "Enviando..." : "Comentar"}
          </button>
        </form>
      </div>
    </div>
  );
}
