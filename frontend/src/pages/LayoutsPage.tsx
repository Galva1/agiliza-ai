import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import {
  createModeloTexto,
  deleteModeloTexto,
  getModelosTexto,
  updateModeloTexto,
  updateModeloTextoStatus,
} from "../api/modelosTextoApi";
import { getApiErrorMessage } from "../api/apiClient";
import { RichTextEditor } from "../components/RichTextEditor";
import { RichTextView } from "../components/RichTextView";
import type { ModeloTexto } from "../types";

export function LayoutsPage() {
  const [modelos, setModelos] = useState<ModeloTexto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [titulo, setTitulo] = useState("");
  const [conteudo, setConteudo] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const load = async () => {
    setIsLoading(true);
    setError(null);
    try {
      setModelos(await getModelosTexto());
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível carregar os layouts."));
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const resetForm = () => {
    setShowForm(false);
    setEditingId(null);
    setTitulo("");
    setConteudo("");
  };

  const startCreating = () => {
    resetForm();
    setShowForm(true);
  };

  const startEditing = (modelo: ModeloTexto) => {
    setEditingId(modelo.id);
    setTitulo(modelo.titulo);
    setConteudo(modelo.conteudo);
    setShowForm(true);
  };

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();
    setIsSubmitting(true);
    setError(null);
    try {
      if (editingId) {
        const updated = await updateModeloTexto(editingId, { titulo, conteudo });
        setModelos((prev) => prev.map((m) => (m.id === editingId ? updated : m)));
      } else {
        await createModeloTexto({ titulo, conteudo });
        await load();
      }
      resetForm();
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível salvar o layout."));
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleToggleStatus = async (modelo: ModeloTexto) => {
    setError(null);
    try {
      const updated = await updateModeloTextoStatus(modelo.id, !modelo.ativo);
      setModelos((prev) => prev.map((m) => (m.id === modelo.id ? updated : m)));
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível atualizar o status do layout."));
    }
  };

  const handleDelete = async (modelo: ModeloTexto) => {
    if (!window.confirm(`Excluir o layout "${modelo.titulo}"?`)) return;
    setError(null);
    try {
      await deleteModeloTexto(modelo.id);
      setModelos((prev) => prev.filter((m) => m.id !== modelo.id));
      if (editingId === modelo.id) resetForm();
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível excluir o layout."));
    }
  };

  return (
    <div className="flex flex-col gap-6">
      <div className="flex items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-surface-900 dark:text-white">Layouts de texto</h1>
          <p className="text-sm text-surface-500 dark:text-surface-400">
            Modelos que técnicos e solicitantes podem aplicar ao registrar acompanhamentos no chamado.
          </p>
        </div>
        <button type="button" className="btn-primary" onClick={showForm ? resetForm : startCreating}>
          {showForm ? "Cancelar" : "Novo layout"}
        </button>
      </div>

      {error && <div className="alert-error">{error}</div>}

      {showForm && (
        <form className="card flex flex-col gap-4" onSubmit={handleSubmit}>
          <h2 className="text-lg font-semibold text-surface-900 dark:text-white">
            {editingId ? "Editar layout" : "Novo layout"}
          </h2>
          <label className="flex flex-col gap-1.5">
            <span className="field-label">Título</span>
            <input
              className="field-input"
              value={titulo}
              onChange={(e) => setTitulo(e.target.value)}
              required
              maxLength={120}
            />
          </label>
          <div className="flex flex-col gap-1.5">
            <span className="field-label">Conteúdo</span>
            <RichTextEditor value={conteudo} onChange={setConteudo} placeholder="Escreva o texto do layout..." />
          </div>
          <button type="submit" className="btn-primary self-start" disabled={isSubmitting}>
            {isSubmitting ? "Salvando..." : editingId ? "Salvar layout" : "Criar layout"}
          </button>
        </form>
      )}

      {isLoading ? (
        <p className="text-surface-500 dark:text-surface-400">Carregando layouts...</p>
      ) : modelos.length === 0 ? (
        <p className="text-surface-500 dark:text-surface-400">Nenhum layout cadastrado.</p>
      ) : (
        <ul className="grid grid-cols-1 gap-4 md:grid-cols-2">
          {modelos.map((modelo) => (
            <li key={modelo.id} className="card flex flex-col gap-3">
              <div className="flex items-center justify-between gap-2">
                <strong className="text-surface-900 dark:text-surface-100">{modelo.titulo}</strong>
                <span
                  className={`inline-flex min-w-[80px] items-center justify-center rounded-full px-2.5 py-1 text-center text-xs font-semibold ${
                    modelo.ativo
                      ? "bg-emerald-100 text-emerald-700 dark:bg-emerald-950/60 dark:text-emerald-300"
                      : "bg-surface-200 text-surface-600 dark:bg-surface-800 dark:text-surface-300"
                  }`}
                >
                  {modelo.ativo ? "Ativo" : "Inativo"}
                </span>
              </div>
              <div className="max-h-40 overflow-hidden">
                <RichTextView html={modelo.conteudo} />
              </div>
              <div className="mt-auto flex flex-wrap gap-2">
                <button type="button" className="action-btn btn-secondary" onClick={() => startEditing(modelo)}>
                  Editar
                </button>
                <button
                  type="button"
                  className={`action-btn ${modelo.ativo ? "btn-secondary" : "btn-primary"}`}
                  onClick={() => handleToggleStatus(modelo)}
                >
                  {modelo.ativo ? "Desativar" : "Ativar"}
                </button>
                <button type="button" className="action-btn btn-danger" onClick={() => handleDelete(modelo)}>
                  Excluir
                </button>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
