import { Navigate, Route, Routes } from "react-router-dom";
import { Layout } from "./components/Layout";
import { ProtectedRoute } from "./components/ProtectedRoute";
import { LoginPage } from "./pages/LoginPage";
import { PainelPage } from "./pages/PainelPage";
import { ChamadoDetailPage } from "./pages/ChamadoDetailPage";
import { UserSettingsPage } from "./pages/UserSettingsPage";
import { CategoriasPage } from "./pages/CategoriasPage";
import { ProfilePage } from "./pages/ProfilePage";

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />

      <Route
        element={
          <ProtectedRoute>
            <Layout />
          </ProtectedRoute>
        }
      >
        <Route path="/" element={<PainelPage />} />
        <Route path="/chamados/:id" element={<ChamadoDetailPage />} />
        <Route path="/perfil" element={<ProfilePage />} />
        <Route
          path="/categorias"
          element={
            <ProtectedRoute allowedRoles={["Administrador"]}>
              <CategoriasPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/usuarios"
          element={
            <ProtectedRoute allowedRoles={["Administrador"]}>
              <UserSettingsPage />
            </ProtectedRoute>
          }
        />
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}

export default App;
