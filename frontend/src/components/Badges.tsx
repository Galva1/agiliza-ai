import type { TicketPriority, TicketStatus } from "../types";

const STATUS_LABELS: Record<TicketStatus, string> = {
  Aberto: "Aberto",
  EmAndamento: "Em andamento",
  Aguardando: "Aguardando",
  Resolvido: "Resolvido",
  Fechado: "Fechado",
};

const PRIORITY_LABELS: Record<TicketPriority, string> = {
  Baixa: "Baixa",
  Media: "Média",
  Alta: "Alta",
  Urgente: "Urgente",
};

export function StatusBadge({ status }: { status: TicketStatus }) {
  return <span className={`badge badge-status-${status}`}>{STATUS_LABELS[status]}</span>;
}

export function PriorityBadge({ priority }: { priority: TicketPriority }) {
  return <span className={`badge badge-priority-${priority}`}>{PRIORITY_LABELS[priority]}</span>;
}

export const ALL_STATUSES: TicketStatus[] = [
  "Aberto",
  "EmAndamento",
  "Aguardando",
  "Resolvido",
  "Fechado",
];

export const ALL_PRIORITIES: TicketPriority[] = ["Baixa", "Media", "Alta", "Urgente"];
