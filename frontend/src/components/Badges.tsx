import type { PrioridadeChamado, StatusChamado } from "../types";

const STATUS_LABELS: Record<StatusChamado, string> = {
  Aberto: "Aberto",
  EmAndamento: "Em andamento",
  Aguardando: "Aguardando",
  Resolvido: "Resolvido",
  Fechado: "Fechado",
};

const STATUS_CLASSES: Record<StatusChamado, string> = {
  Aberto: "bg-blue-100 text-blue-700 dark:bg-blue-950/60 dark:text-blue-300",
  EmAndamento: "bg-amber-100 text-amber-700 dark:bg-amber-950/60 dark:text-amber-300",
  Aguardando: "bg-violet-100 text-violet-700 dark:bg-violet-950/60 dark:text-violet-300",
  Resolvido: "bg-emerald-100 text-emerald-700 dark:bg-emerald-950/60 dark:text-emerald-300",
  Fechado: "bg-surface-200 text-surface-600 dark:bg-surface-800 dark:text-surface-300",
};

const PRIORITY_LABELS: Record<PrioridadeChamado, string> = {
  Baixa: "Baixa",
  Media: "Média",
  Alta: "Alta",
  Urgente: "Urgente",
};

const PRIORITY_CLASSES: Record<PrioridadeChamado, string> = {
  Baixa: "bg-surface-200 text-surface-600 dark:bg-surface-800 dark:text-surface-300",
  Media: "bg-blue-100 text-blue-700 dark:bg-blue-950/60 dark:text-blue-300",
  Alta: "bg-amber-100 text-amber-700 dark:bg-amber-950/60 dark:text-amber-300",
  Urgente: "bg-red-100 text-red-700 dark:bg-red-950/60 dark:text-red-300",
};

export function StatusBadge({ status }: { status: StatusChamado }) {
  return <span className={`badge-status ${STATUS_CLASSES[status]}`}>{STATUS_LABELS[status]}</span>;
}

export function PriorityBadge({ priority }: { priority: PrioridadeChamado }) {
  return <span className={`badge-priority ${PRIORITY_CLASSES[priority]}`}>{PRIORITY_LABELS[priority]}</span>;
}

export const ALL_STATUSES: StatusChamado[] = [
  "Aberto",
  "EmAndamento",
  "Aguardando",
  "Resolvido",
  "Fechado",
];

export const ALL_PRIORITIES: PrioridadeChamado[] = ["Baixa", "Media", "Alta", "Urgente"];

export const STATUS_LABEL_MAP = STATUS_LABELS;
export const PRIORITY_LABEL_MAP = PRIORITY_LABELS;
