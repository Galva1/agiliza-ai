import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import { createUser, getAllUsers, updateUserRole, updateUserStatus } from "../api/usersApi";
import { getApiErrorMessage } from "../api/apiClient";
import type { User, UserRole } from "../types";

const ROLES: UserRole[] = ["Admin", "Agente", "Solicitante"];

export function UserSettingsPage() {
  const [users, setUsers] = useState<User[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [role, setRole] = useState<UserRole>("Solicitante");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const load = async () => {
    setIsLoading(true);
    setError(null);
    try {
      setUsers(await getAllUsers());
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível carregar os usuários."));
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const handleCreate = async (event: FormEvent) => {
    event.preventDefault();
    setIsSubmitting(true);
    setError(null);
    try {
      await createUser({ name, email, password, role });
      setName("");
      setEmail("");
      setPassword("");
      setRole("Solicitante");
      setShowForm(false);
      await load();
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível criar o usuário."));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleRoleChange = async (userId: string, newRole: UserRole) => {
    setError(null);
    try {
      const updated = await updateUserRole(userId, newRole);
      setUsers((prev) => prev.map((u) => (u.id === userId ? updated : u)));
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível atualizar o perfil."));
    }
  };

  const handleStatusToggle = async (userId: string, isActive: boolean) => {
    setError(null);
    try {
      const updated = await updateUserStatus(userId, isActive);
      setUsers((prev) => prev.map((u) => (u.id === userId ? updated : u)));
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível atualizar o status."));
    }
  };

  return (
    <div className="page">
      <div className="page-header">
        <h1>Usuários</h1>
        <button type="button" className="btn btn-primary" onClick={() => setShowForm((v) => !v)}>
          {showForm ? "Cancelar" : "Novo usuário"}
        </button>
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      {showForm && (
        <form className="card form" onSubmit={handleCreate}>
          <div className="form-row">
            <label className="field">
              <span>Nome</span>
              <input value={name} onChange={(e) => setName(e.target.value)} required />
            </label>
            <label className="field">
              <span>E-mail</span>
              <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
            </label>
          </div>
          <div className="form-row">
            <label className="field">
              <span>Senha provisória</span>
              <input
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
                minLength={6}
              />
            </label>
            <label className="field">
              <span>Perfil</span>
              <select value={role} onChange={(e) => setRole(e.target.value as UserRole)}>
                {ROLES.map((r) => (
                  <option key={r} value={r}>
                    {r}
                  </option>
                ))}
              </select>
            </label>
          </div>
          <button type="submit" className="btn btn-primary" disabled={isSubmitting}>
            {isSubmitting ? "Criando..." : "Criar usuário"}
          </button>
        </form>
      )}

      {isLoading ? (
        <p>Carregando usuários...</p>
      ) : (
        <table className="table">
          <thead>
            <tr>
              <th>Nome</th>
              <th>E-mail</th>
              <th>Perfil</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            {users.map((u) => (
              <tr key={u.id}>
                <td>{u.name}</td>
                <td>{u.email}</td>
                <td>
                  <select value={u.role} onChange={(e) => handleRoleChange(u.id, e.target.value as UserRole)}>
                    {ROLES.map((r) => (
                      <option key={r} value={r}>
                        {r}
                      </option>
                    ))}
                  </select>
                </td>
                <td>
                  <button
                    type="button"
                    className={`btn ${u.isActive ? "btn-ghost" : "btn-primary"}`}
                    onClick={() => handleStatusToggle(u.id, !u.isActive)}
                  >
                    {u.isActive ? "Ativo" : "Inativo"}
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
