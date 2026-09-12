import { NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export function Navbar() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <header className="navbar">
      <div className="navbar-brand">Agiliza</div>
      <nav className="navbar-links">
        <NavLink to="/" end>
          Chamados
        </NavLink>
        {user?.role === "Admin" && <NavLink to="/usuarios">Usuários</NavLink>}
        <NavLink to="/perfil">Meu perfil</NavLink>
      </nav>
      <div className="navbar-user">
        <span>
          {user?.name} <small>({user?.role})</small>
        </span>
        <button type="button" onClick={handleLogout} className="btn btn-ghost">
          Sair
        </button>
      </div>
    </header>
  );
}
