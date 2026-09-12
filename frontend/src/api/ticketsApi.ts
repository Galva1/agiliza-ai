import { apiClient } from "./apiClient";
import type { Ticket, TicketDetail, TicketComment, TicketPriority, TicketStatus } from "../types";

export async function getTickets(status?: TicketStatus): Promise<Ticket[]> {
  const { data } = await apiClient.get<Ticket[]>("/tickets", {
    params: status ? { status } : undefined,
  });
  return data;
}

export async function getTicketById(id: string): Promise<TicketDetail> {
  const { data } = await apiClient.get<TicketDetail>(`/tickets/${id}`);
  return data;
}

export async function createTicket(payload: {
  title: string;
  description: string;
  priority: TicketPriority;
}): Promise<Ticket> {
  const { data } = await apiClient.post<Ticket>("/tickets", payload);
  return data;
}

export async function updateTicket(
  id: string,
  payload: Partial<{ status: TicketStatus; priority: TicketPriority; assigneeId: string }>
): Promise<Ticket> {
  const { data } = await apiClient.put<Ticket>(`/tickets/${id}`, payload);
  return data;
}

export async function addTicketComment(id: string, message: string): Promise<TicketComment> {
  const { data } = await apiClient.post<TicketComment>(`/tickets/${id}/comments`, { message });
  return data;
}
