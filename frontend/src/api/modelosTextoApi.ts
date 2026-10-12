import { apiClient } from "./apiClient";
import type { ModeloTexto } from "../types";

export async function getModelosTexto(): Promise<ModeloTexto[]> {
  const { data } = await apiClient.get<ModeloTexto[]>("/modelos-texto");
  return data;
}

export async function createModeloTexto(payload: { titulo: string; conteudo: string }): Promise<ModeloTexto> {
  const { data } = await apiClient.post<ModeloTexto>("/modelos-texto", payload);
  return data;
}

export async function updateModeloTexto(
  id: string,
  payload: { titulo: string; conteudo: string }
): Promise<ModeloTexto> {
  const { data } = await apiClient.put<ModeloTexto>(`/modelos-texto/${id}`, payload);
  return data;
}

export async function updateModeloTextoStatus(id: string, ativo: boolean): Promise<ModeloTexto> {
  const { data } = await apiClient.put<ModeloTexto>(`/modelos-texto/${id}/status`, { ativo });
  return data;
}

export async function deleteModeloTexto(id: string): Promise<void> {
  await apiClient.delete(`/modelos-texto/${id}`);
}
