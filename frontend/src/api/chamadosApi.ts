import { apiClient } from "./apiClient";
import type {
  Chamado,
  ChamadoDetalhe,
  ComentarioChamado,
  PainelChamados,
  PrioridadeChamado,
  StatusChamado,
} from "../types";

export async function getChamados(params?: {
  status?: StatusChamado;
  semAtribuicao?: boolean;
  categoriaId?: string;
}): Promise<Chamado[]> {
  const { data } = await apiClient.get<Chamado[]>("/chamados", { params });
  return data;
}

export async function getPainelChamados(params: {
  tamanhoPagina?: number;
  paginaAbertos?: number;
  paginaAtribuidos?: number;
  paginaConcluidos?: number;
}): Promise<PainelChamados> {
  const { data } = await apiClient.get<PainelChamados>("/chamados/painel", { params });
  return data;
}

export async function getChamadoById(id: string): Promise<ChamadoDetalhe> {
  const { data } = await apiClient.get<ChamadoDetalhe>(`/chamados/${id}`);
  return data;
}

export async function createChamado(payload: {
  titulo: string;
  descricao: string;
  prioridade: PrioridadeChamado;
  categoriaId?: string;
}): Promise<Chamado> {
  const { data } = await apiClient.post<Chamado>("/chamados", payload);
  return data;
}

export async function updateChamado(
  id: string,
  payload: Partial<{
    status: StatusChamado;
    prioridade: PrioridadeChamado;
    categoriaId: string | null;
    tecnicoId: string | null;
  }>
): Promise<Chamado> {
  const { data } = await apiClient.put<Chamado>(`/chamados/${id}`, payload);
  return data;
}

export async function addChamadoComentario(id: string, mensagem: string): Promise<ComentarioChamado> {
  const { data } = await apiClient.post<ComentarioChamado>(`/chamados/${id}/comentarios`, { mensagem });
  return data;
}

export async function proporResolucao(id: string, resolucao: string): Promise<Chamado> {
  const { data } = await apiClient.post<Chamado>(`/chamados/${id}/resolucao`, { resolucao });
  return data;
}

export async function aprovarResolucao(id: string): Promise<Chamado> {
  const { data } = await apiClient.post<Chamado>(`/chamados/${id}/resolucao/aprovar`);
  return data;
}
