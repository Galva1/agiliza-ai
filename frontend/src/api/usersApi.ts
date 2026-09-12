import { apiClient } from "./apiClient";
import type { User, UserRole } from "../types";

export async function getAllUsers(): Promise<User[]> {
  const { data } = await apiClient.get<User[]>("/users");
  return data;
}

export async function getAssignableAgents(): Promise<User[]> {
  const { data } = await apiClient.get<User[]>("/users/agents");
  return data;
}

export async function getMe(): Promise<User> {
  const { data } = await apiClient.get<User>("/users/me");
  return data;
}

export async function updateMe(payload: {
  name: string;
  currentPassword?: string;
  newPassword?: string;
}): Promise<User> {
  const { data } = await apiClient.put<User>("/users/me", payload);
  return data;
}

export async function createUser(payload: {
  name: string;
  email: string;
  password: string;
  role: UserRole;
}): Promise<User> {
  const { data } = await apiClient.post<User>("/users", payload);
  return data;
}

export async function updateUserRole(userId: string, role: UserRole): Promise<User> {
  const { data } = await apiClient.put<User>(`/users/${userId}/role`, { role });
  return data;
}

export async function updateUserStatus(userId: string, isActive: boolean): Promise<User> {
  const { data } = await apiClient.put<User>(`/users/${userId}/status`, { isActive });
  return data;
}
