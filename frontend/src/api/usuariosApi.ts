import { apiClient } from "./apiClient";
import type { PerfilUsuario, Usuario } from "../types";

export async function getAllUsuarios(): Promise<Usuario[]> {
  const { data } = await apiClient.get<Usuario[]>("/usuarios");
  return data;
}

export async function getTecnicos(): Promise<Usuario[]> {
  const { data } = await apiClient.get<Usuario[]>("/usuarios/tecnicos");
  return data;
}

export async function getMe(): Promise<Usuario> {
  const { data } = await apiClient.get<Usuario>("/usuarios/me");
  return data;
}

export async function updateMe(payload: {
  nome: string;
  senhaAtual?: string;
  novaSenha?: string;
}): Promise<Usuario> {
  const { data } = await apiClient.put<Usuario>("/usuarios/me", payload);
  return data;
}

export async function getUsuarioById(id: string): Promise<Usuario> {
  const { data } = await apiClient.get<Usuario>(`/usuarios/${id}`);
  return data;
}

export async function createUsuario(payload: {
  nome: string;
  email: string;
  senha: string;
  perfil: PerfilUsuario;
}): Promise<Usuario> {
  const { data } = await apiClient.post<Usuario>("/usuarios", payload);
  return data;
}

export async function updateUsuarioPerfil(usuarioId: string, perfil: PerfilUsuario): Promise<Usuario> {
  const { data } = await apiClient.put<Usuario>(`/usuarios/${usuarioId}/perfil`, { perfil });
  return data;
}

export async function updateUsuarioStatus(usuarioId: string, ativo: boolean): Promise<Usuario> {
  const { data } = await apiClient.put<Usuario>(`/usuarios/${usuarioId}/status`, { ativo });
  return data;
}
