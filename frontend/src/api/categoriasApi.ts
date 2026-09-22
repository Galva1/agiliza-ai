import { apiClient } from "./apiClient";
import type { Categoria } from "../types";

export async function getCategorias(): Promise<Categoria[]> {
  const { data } = await apiClient.get<Categoria[]>("/categorias");
  return data;
}

export async function createCategoria(payload: { nome: string; descricao?: string }): Promise<Categoria> {
  const { data } = await apiClient.post<Categoria>("/categorias", payload);
  return data;
}

export async function updateCategoria(
  id: string,
  payload: { nome: string; descricao?: string }
): Promise<Categoria> {
  const { data } = await apiClient.put<Categoria>(`/categorias/${id}`, payload);
  return data;
}

export async function updateCategoriaStatus(id: string, ativa: boolean): Promise<Categoria> {
  const { data } = await apiClient.put<Categoria>(`/categorias/${id}/status`, { ativa });
  return data;
}
