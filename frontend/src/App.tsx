import { Navigate, Route, Routes } from "react-router-dom";
import { Layout } from "./components/Layout";
import { ProtectedRoute } from "./components/ProtectedRoute";
import { LoginPage } from "./pages/LoginPage";
import { TicketsOverviewPage } from "./pages/TicketsOverviewPage";
import { TicketDetailPage } from "./pages/TicketDetailPage";
import { UserSettingsPage } from "./pages/UserSettingsPage";
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
        <Route path="/" element={<TicketsOverviewPage />} />
        <Route path="/chamados/:id" element={<TicketDetailPage />} />
        <Route path="/perfil" element={<ProfilePage />} />
        <Route
          path="/usuarios"
          element={
            <ProtectedRoute allowedRoles={["Admin"]}>
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
