import { useState } from "react";
import type { FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { getApiErrorMessage } from "../api/apiClient";

export function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [senha, setSenha] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();
    setError(null);
    setIsSubmitting(true);

    try {
      await login(email, senha);
      navigate("/");
    } catch (err) {
      setError(getApiErrorMessage(err, "E-mail ou senha inválidos."));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-gradient-to-br from-brand-50 via-white to-surface-100 px-4 dark:from-surface-950 dark:via-surface-950 dark:to-surface-900">
      <form
        className="w-full max-w-sm rounded-2xl border border-surface-200 bg-white p-8 shadow-lg dark:border-surface-800 dark:bg-surface-900"
        onSubmit={handleSubmit}
      >
        <h1 className="text-2xl font-extrabold text-brand-700 dark:text-brand-300">Agiliza.ai</h1>
        <p className="mt-1 text-sm text-surface-500 dark:text-surface-400">
          Entre com suas credenciais para continuar.
        </p>

        {error && <div className="alert-error mt-4">{error}</div>}

        <label className="mt-5 flex flex-col gap-1.5">
          <span className="field-label">E-mail</span>
          <input
            type="email"
            className="field-input"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            autoFocus
          />
        </label>

        <label className="mt-4 flex flex-col gap-1.5">
          <span className="field-label">Senha</span>
          <input
            type="password"
            className="field-input"
            value={senha}
            onChange={(e) => setSenha(e.target.value)}
            required
          />
        </label>

        <button type="submit" className="btn-primary mt-6 w-full" disabled={isSubmitting}>
          {isSubmitting ? "Entrando..." : "Entrar"}
        </button>
      </form>
    </div>
  );
}
