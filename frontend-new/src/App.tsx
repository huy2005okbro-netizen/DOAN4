import AppRoutes from "./routes/AppRoutes";
import { AuthProvider } from "./shared/tsx/context/AuthContext";

function App() {
  return (
    <AuthProvider>
      <AppRoutes />
    </AuthProvider>
  );
}

export default App;
