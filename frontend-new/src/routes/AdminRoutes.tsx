import { Routes, Route, Navigate } from "react-router-dom";
import ProtectedRoute from "./ProtectedRoute";
import { ROLES } from "../shared/ts/constants/roles";
import AdminLayout from "../admin/tsx/layout/AdminLayout";
import TongQuanAdmin from "../admin/tsx/dashboard/TongQuanAdmin";
import DangNhapAdmin from "../admin/tsx/auth/DangNhapAdmin";
import QuenMatKhauAdmin from "../admin/tsx/auth/QuenMatKhauAdmin";

export default function AdminRoutes() {
  return (
    <Routes>
      {/* Public auth routes */}
      <Route path="dang-nhap" element={<DangNhapAdmin />} />
      <Route path="quen-mat-khau" element={<QuenMatKhauAdmin />} />

      {/* Protected — bọc AdminLayout */}
      <Route
        element={
          <ProtectedRoute
            allowedRoles={[ROLES.ADMIN]}
            redirectTo="/admin/dang-nhap"
          />
        }
      >
        <Route element={<AdminLayout />}>
          <Route index element={<TongQuanAdmin />} />
          <Route path="tong-quan" element={<TongQuanAdmin />} />
          {/* Các route khác thêm ở đây */}
        </Route>
      </Route>

      {/* Fallback */}
      <Route path="*" element={<Navigate to="/admin/dang-nhap" replace />} />
    </Routes>
  );
}
