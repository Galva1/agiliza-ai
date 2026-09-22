import { useState } from "react";
import type { FormEvent } from "react";
import { useAuth } from "../context/AuthContext";
import { updateMe } from "../api/usuariosApi";
import { getApiErrorMessage } from "../api/apiClient";

const PERFIL_LABELS: Record<string, string> = {
  Administrador: "Administrador",
  Tecnico: "Técnico",
  Solicitante: "Solicitante",
};

export function ProfilePage() {
  const { user, refreshUser } = useAuth();

  const [nome, setNome] = useState(user?.nome ?? "");
  const [senhaAtual, setSenhaAtual] = useState("");
  const [novaSenha, setNovaSenha] = useState("");
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
        nome,
        senhaAtual: senhaAtual || undefined,
        novaSenha: novaSenha || undefined,
      });
      await refreshUser();
      setSenhaAtual("");
      setNovaSenha("");
      setSuccess("Perfil atualizado com sucesso.");
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível atualizar o perfil."));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-2xl font-bold text-surface-900 dark:text-white">Meu perfil</h1>

      <div className="card">
        <dl className="grid grid-cols-[max-content_1fr] gap-x-6 gap-y-2 text-sm">
          <dt className="text-surface-500 dark:text-surface-400">E-mail</dt>
          <dd className="text-surface-900 dark:text-surface-100">{user.email}</dd>
          <dt className="text-surface-500 dark:text-surface-400">Perfil</dt>
          <dd className="text-surface-900 dark:text-surface-100">{PERFIL_LABELS[user.perfil]}</dd>
          <dt className="text-surface-500 dark:text-surface-400">Membro desde</dt>
          <dd className="text-surface-900 dark:text-surface-100">
            {new Date(user.criadoEm).toLocaleDateString("pt-BR")}
          </dd>
        </dl>
      </div>

      <form className="card flex flex-col gap-4" onSubmit={handleSubmit}>
        <h2 className="text-lg font-semibold text-surface-900 dark:text-white">Editar dados</h2>

        {error && <div className="alert-error">{error}</div>}
        {success && <div className="alert-success">{success}</div>}

        <label className="flex flex-col gap-1.5">
          <span className="field-label">Nome</span>
          <input className="field-input" value={nome} onChange={(e) => setNome(e.target.value)} required />
        </label>

        <p className="text-sm text-surface-500 dark:text-surface-400">
          Preencha os campos abaixo apenas se quiser trocar a senha.
        </p>

        <div className="flex flex-col gap-4 sm:flex-row">
          <label className="flex flex-1 flex-col gap-1.5">
            <span className="field-label">Senha atual</span>
            <input
              type="password"
              className="field-input"
              value={senhaAtual}
              onChange={(e) => setSenhaAtual(e.target.value)}
            />
          </label>
          <label className="flex flex-1 flex-col gap-1.5">
            <span className="field-label">Nova senha</span>
            <input
              type="password"
              className="field-input"
              value={novaSenha}
              onChange={(e) => setNovaSenha(e.target.value)}
              minLength={6}
            />
          </label>
        </div>

        <button type="submit" className="btn-primary self-start" disabled={isSubmitting}>
          {isSubmitting ? "Salvando..." : "Salvar alterações"}
        </button>
      </form>
    </div>
  );
}
