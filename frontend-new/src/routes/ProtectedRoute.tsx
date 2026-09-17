import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../shared/ts/hooks/useAuth";

interface Props {
  allowedRoles: string[];
  redirectTo?: string;
}

export default function ProtectedRoute({ allowedRoles, redirectTo }: Props) {
  const { user, isLoading } = useAuth();

  // Chờ auth context load từ localStorage xong
  if (isLoading) {
    return (
      <div
        style={{
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          height: "100vh",
          background: "#f1f5f9",
          flexDirection: "column",
          gap: 12,
        }}
      >
        <div
          style={{
            width: 36,
            height: 36,
            border: "3px solid #e2e8f0",
            borderTopColor: "#2563eb",
            borderRadius: "50%",
            animation: "spin 0.7s linear infinite",
          }}
        />
        <style>{`@keyframes spin { to { transform: rotate(360deg); } }`}</style>
      </div>
    );
  }

  // Chưa đăng nhập → về trang login
  if (!user) {
    return <Navigate to={redirectTo ?? "/customer/dang-nhap"} replace />;
  }

  // So sánh case-insensitive để tránh lỗi "Admin" vs "ADMIN"
  const userRoleUpper = user.role.toUpperCase();
  const allowed = allowedRoles.map((r) => r.toUpperCase());

  if (!allowed.includes(userRoleUpper)) {
    // Đã đăng nhập nhưng sai role → redirect về dashboard đúng
    if (userRoleUpper === "ADMIN") return <Navigate to="/admin" replace />;
    if (userRoleUpper === "EMPLOYEE")
      return <Navigate to="/employee" replace />;
    if (userRoleUpper === "CUSTOMER")
      return <Navigate to="/customer" replace />;
    return <Navigate to="/" replace />;
  }

  return <Outlet />;
}
