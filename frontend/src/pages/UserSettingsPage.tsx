import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import { createUsuario, getAllUsuarios, updateUsuarioPerfil, updateUsuarioStatus } from "../api/usuariosApi";
import { getApiErrorMessage } from "../api/apiClient";
import type { PerfilUsuario, Usuario } from "../types";

const PERFIS: PerfilUsuario[] = ["Administrador", "Tecnico", "Solicitante"];

const PERFIL_LABELS: Record<PerfilUsuario, string> = {
  Administrador: "Administrador",
  Tecnico: "Técnico",
  Solicitante: "Solicitante",
};

export function UserSettingsPage() {
  const [users, setUsers] = useState<Usuario[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);
  const [nome, setNome] = useState("");
  const [email, setEmail] = useState("");
  const [senha, setSenha] = useState("");
  const [perfil, setPerfil] = useState<PerfilUsuario>("Solicitante");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const load = async () => {
    setIsLoading(true);
    setError(null);
    try {
      setUsers(await getAllUsuarios());
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
      await createUsuario({ nome, email, senha, perfil });
      setNome("");
      setEmail("");
      setSenha("");
      setPerfil("Solicitante");
      setShowForm(false);
      await load();
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível criar o usuário."));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handlePerfilChange = async (userId: string, newPerfil: PerfilUsuario) => {
    setError(null);
    try {
      const updated = await updateUsuarioPerfil(userId, newPerfil);
      setUsers((prev) => prev.map((u) => (u.id === userId ? updated : u)));
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível atualizar o perfil."));
    }
  };

  const handleStatusToggle = async (userId: string, ativo: boolean) => {
    setError(null);
    try {
      const updated = await updateUsuarioStatus(userId, ativo);
      setUsers((prev) => prev.map((u) => (u.id === userId ? updated : u)));
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível atualizar o status."));
    }
  };

  return (
    <div className="flex flex-col gap-6">
      <div className="flex items-center justify-between gap-4">
        <h1 className="text-2xl font-bold text-surface-900 dark:text-white">Usuários</h1>
        <button type="button" className="btn-primary" onClick={() => setShowForm((v) => !v)}>
          {showForm ? "Cancelar" : "Novo usuário"}
        </button>
      </div>

      {error && <div className="alert-error">{error}</div>}

      {showForm && (
        <form className="card flex flex-col gap-4" onSubmit={handleCreate}>
          <div className="flex flex-col gap-4 sm:flex-row">
            <label className="flex flex-1 flex-col gap-1.5">
              <span className="field-label">Nome</span>
              <input className="field-input" value={nome} onChange={(e) => setNome(e.target.value)} required />
            </label>
            <label className="flex flex-1 flex-col gap-1.5">
              <span className="field-label">E-mail</span>
              <input
                type="email"
                className="field-input"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </label>
          </div>
          <div className="flex flex-col gap-4 sm:flex-row">
            <label className="flex flex-1 flex-col gap-1.5">
              <span className="field-label">Senha provisória</span>
              <input
                type="password"
                className="field-input"
                value={senha}
                onChange={(e) => setSenha(e.target.value)}
                required
                minLength={6}
              />
            </label>
            <label className="flex flex-1 flex-col gap-1.5">
              <span className="field-label">Perfil</span>
              <select
                className="field-input"
                value={perfil}
                onChange={(e) => setPerfil(e.target.value as PerfilUsuario)}
              >
                {PERFIS.map((p) => (
                  <option key={p} value={p}>
                    {PERFIL_LABELS[p]}
                  </option>
                ))}
              </select>
            </label>
          </div>
          <button type="submit" className="btn-primary self-start" disabled={isSubmitting}>
            {isSubmitting ? "Criando..." : "Criar usuário"}
          </button>
        </form>
      )}

      {isLoading ? (
        <p className="text-surface-500 dark:text-surface-400">Carregando usuários...</p>
      ) : (
        <div className="table-shell">
          <table className="data-table">
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
                  <td className="font-medium text-surface-900 dark:text-surface-100">{u.nome}</td>
                  <td className="text-surface-600 dark:text-surface-300">{u.email}</td>
                  <td>
                    <select
                      className="field-input"
                      value={u.perfil}
                      onChange={(e) => handlePerfilChange(u.id, e.target.value as PerfilUsuario)}
                    >
                      {PERFIS.map((p) => (
                        <option key={p} value={p}>
                          {PERFIL_LABELS[p]}
                        </option>
                      ))}
                    </select>
                  </td>
                  <td>
                    <button
                      type="button"
                      className={`action-btn ${u.ativo ? "btn-secondary" : "btn-primary"}`}
                      onClick={() => handleStatusToggle(u.id, !u.ativo)}
                    >
                      {u.ativo ? "Ativo" : "Inativo"}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
