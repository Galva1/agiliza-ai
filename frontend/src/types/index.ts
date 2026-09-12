// Estes tipos espelham os DTOs do backend (HelpDesk.Api/DTOs).
// Os enums são transmitidos pela API como texto (JsonStringEnumConverter),
// então os valores aqui são strings, não números.

export type UserRole = "Admin" | "Agente" | "Solicitante";

export type TicketStatus =
  | "Aberto"
  | "EmAndamento"
  | "Aguardando"
  | "Resolvido"
  | "Fechado";

export type TicketPriority = "Baixa" | "Media" | "Alta" | "Urgente";

export interface User {
  id: string;
  name: string;
  email: string;
  role: UserRole;
  isActive: boolean;
  createdAt: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
  user: User;
}

export interface Ticket {
  id: string;
  title: string;
  description: string;
  status: TicketStatus;
  priority: TicketPriority;
  requesterId: string;
  requesterName: string;
  assigneeId: string | null;
  assigneeName: string | null;
  createdAt: string;
  updatedAt: string | null;
  closedAt: string | null;
}

export interface TicketComment {
  id: string;
  userId: string;
  userName: string;
  message: string;
  createdAt: string;
}

export interface TicketDetail extends Ticket {
  comments: TicketComment[];
}

export interface ApiErrorBody {
  status: number;
  message: string;
}
