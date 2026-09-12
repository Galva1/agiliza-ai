import { useState } from "react";
import type { FormEvent } from "react";
import { useAuth } from "../context/AuthContext";
import { updateMe } from "../api/usersApi";
import { getApiErrorMessage } from "../api/apiClient";

export function ProfilePage() {
  const { user, refreshUser } = useAuth();

  const [name, setName] = useState(user?.name ?? "");
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  if (!user) return null;

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();
    setError(null);
    setSuccess(null);
    setIsSubmitting(true);

    try {
      await updateMe({
        name,
        currentPassword: currentPassword || undefined,
        newPassword: newPassword || undefined,
      });
      await refreshUser();
      setCurrentPassword("");
      setNewPassword("");
      setSuccess("Perfil atualizado com sucesso.");
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível atualizar o perfil."));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="page">
      <h1>Meu perfil</h1>

      <div className="card">
        <dl className="ticket-meta">
          <dt>E-mail</dt>
          <dd>{user.email}</dd>
          <dt>Perfil</dt>
          <dd>{user.role}</dd>
          <dt>Membro desde</dt>
          <dd>{new Date(user.createdAt).toLocaleDateString("pt-BR")}</dd>
        </dl>
      </div>

      <form className="card form" onSubmit={handleSubmit}>
        <h2>Editar dados</h2>

        {error && <div className="alert alert-error">{error}</div>}
        {success && <div className="alert alert-success">{success}</div>}

        <label className="field">
          <span>Nome</span>
          <input value={name} onChange={(e) => setName(e.target.value)} required />
        </label>

        <p className="text-muted">Preencha os campos abaixo apenas se quiser trocar a senha.</p>

        <div className="form-row">
          <label className="field">
            <span>Senha atual</span>
            <input
              type="password"
              value={currentPassword}
              onChange={(e) => setCurrentPassword(e.target.value)}
            />
          </label>
          <label className="field">
            <span>Nova senha</span>
            <input
              type="password"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              minLength={6}
            />
          </label>
        </div>

        <button type="submit" className="btn btn-primary" disabled={isSubmitting}>
          {isSubmitting ? "Salvando..." : "Salvar alterações"}
        </button>
      </form>
    </div>
  );
}
