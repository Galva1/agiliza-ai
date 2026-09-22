import { apiClient } from "./apiClient";
import type { LoginResponse } from "../types";

export async function login(email: string, senha: string): Promise<LoginResponse> {
  const { data } = await apiClient.post<LoginResponse>("/autenticacao/login", { email, senha });
  return data;
}
