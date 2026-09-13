import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../shared/ts/hooks/useAuth";

export default function GuestRoute() {
  const { user, isLoading } = useAuth();

  if (isLoading) return null;

  if (user) return <Navigate to="/" replace />;

  return <Outlet />;
}
