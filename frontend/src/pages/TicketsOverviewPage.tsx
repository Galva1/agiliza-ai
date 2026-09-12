import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import { Link } from "react-router-dom";
import { createTicket, getTickets } from "../api/ticketsApi";
import { getApiErrorMessage } from "../api/apiClient";
import { ALL_PRIORITIES, ALL_STATUSES, PriorityBadge, StatusBadge } from "../components/Badges";
import { useAuth } from "../context/AuthContext";
import type { Ticket, TicketPriority, TicketStatus } from "../types";

export function TicketsOverviewPage() {
  const { user } = useAuth();
  const [tickets, setTickets] = useState<Ticket[]>([]);
  const [statusFilter, setStatusFilter] = useState<TicketStatus | "">("");
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [priority, setPriority] = useState<TicketPriority>("Media");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const loadTickets = async () => {
    setIsLoading(true);
    setError(null);
    try {
      const data = await getTickets(statusFilter || undefined);
      setTickets(data);
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível carregar os chamados."));
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadTickets();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [statusFilter]);

  const handleCreate = async (event: FormEvent) => {
    event.preventDefault();
    setIsSubmitting(true);
    setError(null);
    try {
      await createTicket({ title, description, priority });
      setTitle("");
      setDescription("");
      setPriority("Media");
      setShowForm(false);
      await loadTickets();
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível abrir o chamado."));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="page">
      <div className="page-header">
        <h1>Chamados</h1>
        <button type="button" className="btn btn-primary" onClick={() => setShowForm((v) => !v)}>
          {showForm ? "Cancelar" : "Novo chamado"}
        </button>
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      {showForm && (
        <form className="card form" onSubmit={handleCreate}>
          <label className="field">
            <span>Título</span>
            <input value={title} onChange={(e) => setTitle(e.target.value)} required maxLength={200} />
          </label>
          <label className="field">
            <span>Descrição</span>
            <textarea
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              required
              rows={4}
            />
          </label>
          <label className="field">
            <span>Prioridade</span>
            <select value={priority} onChange={(e) => setPriority(e.target.value as TicketPriority)}>
              {ALL_PRIORITIES.map((p) => (
                <option key={p} value={p}>
                  {p}
                </option>
              ))}
            </select>
          </label>
          <button type="submit" className="btn btn-primary" disabled={isSubmitting}>
            {isSubmitting ? "Enviando..." : "Abrir chamado"}
          </button>
        </form>
      )}

      <div className="filters">
        <label>
          Status:
          <select value={statusFilter} onChange={(e) => setStatusFilter(e.target.value as TicketStatus | "")}>
            <option value="">Todos</option>
            {ALL_STATUSES.map((s) => (
              <option key={s} value={s}>
                {s}
              </option>
            ))}
          </select>
        </label>
      </div>

      {isLoading ? (
        <p>Carregando chamados...</p>
      ) : tickets.length === 0 ? (
        <p className="text-muted">Nenhum chamado encontrado.</p>
      ) : (
        <table className="table">
          <thead>
            <tr>
              <th>Título</th>
              <th>Status</th>
              <th>Prioridade</th>
              {user?.role !== "Solicitante" && <th>Solicitante</th>}
              <th>Responsável</th>
              <th>Criado em</th>
            </tr>
          </thead>
          <tbody>
            {tickets.map((ticket) => (
              <tr key={ticket.id}>
                <td>
                  <Link to={`/chamados/${ticket.id}`}>{ticket.title}</Link>
                </td>
                <td>
                  <StatusBadge status={ticket.status} />
                </td>
                <td>
                  <PriorityBadge priority={ticket.priority} />
                </td>
                {user?.role !== "Solicitante" && <td>{ticket.requesterName}</td>}
                <td>{ticket.assigneeName ?? "—"}</td>
                <td>{new Date(ticket.createdAt).toLocaleString("pt-BR")}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
