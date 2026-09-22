import { NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { useTheme } from "../context/ThemeContext";

const PERFIL_LABELS: Record<string, string> = {
  Administrador: "Administrador",
  Tecnico: "Técnico",
  Solicitante: "Solicitante",
};

export function Navbar() {
  const { user, logout } = useAuth();
  const { theme, toggleTheme } = useTheme();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  const linkClass = ({ isActive }: { isActive: boolean }) =>
    `rounded-lg px-3 py-2 text-sm font-medium transition-colors ${
      isActive
        ? "bg-brand-50 text-brand-700 dark:bg-brand-500/15 dark:text-brand-300"
        : "text-surface-600 hover:bg-surface-100 hover:text-surface-900 dark:text-surface-300 dark:hover:bg-surface-800 dark:hover:text-white"
    }`;

  return (
    <header className="border-b border-surface-200 bg-white/90 backdrop-blur dark:border-surface-800 dark:bg-surface-900/90">
      <div className="mx-auto flex w-full max-w-6xl items-center gap-4 px-4 py-3 sm:px-6 lg:px-8">
        <span className="text-lg font-extrabold tracking-tight text-brand-700 dark:text-brand-300">
          Agiliza.ai
        </span>

        <nav className="flex flex-1 items-center gap-1 overflow-x-auto">
          <NavLink to="/" end className={linkClass}>
            Painel
          </NavLink>
          {user?.perfil === "Administrador" && (
            <NavLink to="/categorias" className={linkClass}>
              Categorias
            </NavLink>
          )}
          {user?.perfil === "Administrador" && (
            <NavLink to="/usuarios" className={linkClass}>
              Usuários
            </NavLink>
          )}
          <NavLink to="/perfil" className={linkClass}>
            Meu perfil
          </NavLink>
        </nav>

        <button
          type="button"
          onClick={toggleTheme}
          aria-label="Alternar tema"
          className="flex h-9 w-9 items-center justify-center rounded-lg border border-surface-300 text-surface-600 transition-colors hover:bg-surface-100 dark:border-surface-700 dark:text-surface-300 dark:hover:bg-surface-800"
        >
          {theme === "dark" ? (
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" className="h-5 w-5">
              <path
                fillRule="evenodd"
                d="M12 2.25a.75.75 0 01.75.75v2.25a.75.75 0 01-1.5 0V3a.75.75 0 01.75-.75zM7.5 12a4.5 4.5 0 119 0 4.5 4.5 0 01-9 0zM18.894 6.166a.75.75 0 00-1.06-1.06l-1.591 1.59a.75.75 0 101.06 1.061l1.591-1.59zM21.75 12a.75.75 0 01-.75.75h-2.25a.75.75 0 010-1.5H21a.75.75 0 01.75.75zM17.834 18.894a.75.75 0 001.06-1.06l-1.59-1.591a.75.75 0 10-1.061 1.06l1.59 1.591zM12 18a.75.75 0 01.75.75V21a.75.75 0 01-1.5 0v-2.25A.75.75 0 0112 18zM7.758 17.303a.75.75 0 00-1.061-1.06l-1.591 1.59a.75.75 0 001.06 1.061l1.591-1.59zM6 12a.75.75 0 01-.75.75H3a.75.75 0 010-1.5h2.25A.75.75 0 016 12zM6.697 7.757a.75.75 0 001.06-1.06l-1.59-1.591a.75.75 0 00-1.061 1.06l1.59 1.591z"
                clipRule="evenodd"
              />
            </svg>
          ) : (
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" className="h-5 w-5">
              <path
                fillRule="evenodd"
                d="M9.528 1.718a.75.75 0 01.162.819A8.97 8.97 0 009 6a9 9 0 009 9 8.97 8.97 0 003.463-.69.75.75 0 01.981.98 10.503 10.503 0 01-9.694 6.46c-5.799 0-10.5-4.7-10.5-10.5 0-4.368 2.667-8.112 6.46-9.694a.75.75 0 01.818.162z"
                clipRule="evenodd"
              />
            </svg>
          )}
        </button>

        <div className="hidden flex-col items-end text-sm sm:flex">
          <span className="font-medium text-surface-800 dark:text-surface-100">{user?.nome}</span>
          <span className="text-xs text-surface-500 dark:text-surface-400">
            {user ? PERFIL_LABELS[user.perfil] : ""}
          </span>
        </div>

        <button type="button" onClick={handleLogout} className="btn-ghost">
          Sair
        </button>
      </div>
    </header>
  );
}
