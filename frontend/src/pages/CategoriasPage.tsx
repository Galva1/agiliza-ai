import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import { createCategoria, getCategorias, updateCategoria, updateCategoriaStatus } from "../api/categoriasApi";
import { getApiErrorMessage } from "../api/apiClient";
import type { Categoria } from "../types";

export function CategoriasPage() {
  const [categorias, setCategorias] = useState<Categoria[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);
  const [nome, setNome] = useState("");
  const [descricao, setDescricao] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const [editingId, setEditingId] = useState<string | null>(null);
  const [editNome, setEditNome] = useState("");
  const [editDescricao, setEditDescricao] = useState("");
  const [isSavingEdit, setIsSavingEdit] = useState(false);

  const load = async () => {
    setIsLoading(true);
    setError(null);
    try {
      setCategorias(await getCategorias());
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível carregar as categorias."));
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
      await createCategoria({ nome, descricao: descricao || undefined });
      setNome("");
      setDescricao("");
      setShowForm(false);
      await load();
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível criar a categoria."));
    } finally {
      setIsSubmitting(false);
    }
  };

  const startEditing = (categoria: Categoria) => {
    setEditingId(categoria.id);
    setEditNome(categoria.nome);
    setEditDescricao(categoria.descricao ?? "");
  };

  const cancelEditing = () => {
    setEditingId(null);
    setEditNome("");
    setEditDescricao("");
  };

  const handleSaveEdit = async (id: string) => {
    setIsSavingEdit(true);
    setError(null);
    try {
      const updated = await updateCategoria(id, { nome: editNome, descricao: editDescricao || undefined });
      setCategorias((prev) => prev.map((c) => (c.id === id ? updated : c)));
      cancelEditing();
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível salvar a categoria."));
    } finally {
      setIsSavingEdit(false);
    }
  };

  const handleToggleStatus = async (categoria: Categoria) => {
    setError(null);
    try {
      const updated = await updateCategoriaStatus(categoria.id, !categoria.ativa);
      setCategorias((prev) => prev.map((c) => (c.id === categoria.id ? updated : c)));
    } catch (err) {
      setError(getApiErrorMessage(err, "Não foi possível atualizar o status da categoria."));
    }
  };

  return (
    <div className="flex flex-col gap-6">
      <div className="flex items-center justify-between gap-4">
        <h1 className="text-2xl font-bold text-surface-900 dark:text-white">Categorias</h1>
        <button type="button" className="btn-primary" onClick={() => setShowForm((v) => !v)}>
          {showForm ? "Cancelar" : "Nova categoria"}
        </button>
      </div>

      {error && <div className="alert-error">{error}</div>}

      {showForm && (
        <form className="card flex flex-col gap-4" onSubmit={handleCreate}>
          <label className="flex flex-col gap-1.5">
            <span className="field-label">Nome</span>
            <input className="field-input" value={nome} onChange={(e) => setNome(e.target.value)} required maxLength={100} />
          </label>
          <label className="flex flex-col gap-1.5">
            <span className="field-label">Descrição</span>
            <textarea
              className="field-input"
              value={descricao}
              onChange={(e) => setDescricao(e.target.value)}
              rows={3}
            />
          </label>
          <button type="submit" className="btn-primary self-start" disabled={isSubmitting}>
            {isSubmitting ? "Criando..." : "Criar categoria"}
          </button>
        </form>
      )}

      {isLoading ? (
        <p className="text-surface-500 dark:text-surface-400">Carregando categorias...</p>
      ) : (
        <div className="table-shell">
          <table className="data-table">
            <thead>
              <tr>
                <th>Nome</th>
                <th>Descrição</th>
                <th>Status</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              {categorias.map((categoria) => (
                <tr key={categoria.id}>
                  {editingId === categoria.id ? (
                    <>
                      <td>
                        <input
                          className="field-input"
                          value={editNome}
                          onChange={(e) => setEditNome(e.target.value)}
                        />
                      </td>
                      <td>
                        <input
                          className="field-input"
                          value={editDescricao}
                          onChange={(e) => setEditDescricao(e.target.value)}
                        />
                      </td>
                      <td>
                        <span
                          className={`inline-flex min-w-[80px] items-center justify-center rounded-full px-2.5 py-1 text-center text-xs font-semibold ${
                            categoria.ativa
                              ? "bg-emerald-100 text-emerald-700 dark:bg-emerald-950/60 dark:text-emerald-300"
                              : "bg-surface-200 text-surface-600 dark:bg-surface-800 dark:text-surface-300"
                          }`}
                        >
                          {categoria.ativa ? "Ativa" : "Inativa"}
                        </span>
                      </td>
                      <td>
                        <div className="flex gap-2">
                          <button
                            type="button"
                            className="action-btn btn-primary"
                            disabled={isSavingEdit}
                            onClick={() => handleSaveEdit(categoria.id)}
                          >
                            {isSavingEdit ? "Salvando..." : "Salvar"}
                          </button>
                          <button type="button" className="action-btn btn-secondary" onClick={cancelEditing}>
                            Cancelar
                          </button>
                        </div>
                      </td>
                    </>
                  ) : (
                    <>
                      <td className="font-medium text-surface-900 dark:text-surface-100">{categoria.nome}</td>
                      <td className="text-surface-600 dark:text-surface-300">
                        {categoria.descricao || <span className="text-surface-400">—</span>}
                      </td>
                      <td>
                        <span
                          className={`inline-flex min-w-[80px] items-center justify-center rounded-full px-2.5 py-1 text-center text-xs font-semibold ${
                            categoria.ativa
                              ? "bg-emerald-100 text-emerald-700 dark:bg-emerald-950/60 dark:text-emerald-300"
                              : "bg-surface-200 text-surface-600 dark:bg-surface-800 dark:text-surface-300"
                          }`}
                        >
                          {categoria.ativa ? "Ativa" : "Inativa"}
                        </span>
                      </td>
                      <td>
                        <div className="flex gap-2">
                          <button
                            type="button"
                            className="action-btn btn-secondary"
                            onClick={() => startEditing(categoria)}
                          >
                            Editar
                          </button>
                          <button
                            type="button"
                            className={`action-btn ${categoria.ativa ? "btn-secondary" : "btn-primary"}`}
                            onClick={() => handleToggleStatus(categoria)}
                          >
                            {categoria.ativa ? "Desativar" : "Ativar"}
                          </button>
                        </div>
                      </td>
                    </>
                  )}
                </tr>
              ))}
              {categorias.length === 0 && (
                <tr>
                  <td colSpan={4} className="text-center text-surface-500 dark:text-surface-400">
                    Nenhuma categoria cadastrada.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
