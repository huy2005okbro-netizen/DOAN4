import { Routes, Route } from "react-router-dom";
import ProtectedRoute from "./ProtectedRoute";
import { ROLES } from "../shared/ts/constants/roles";
import AdminLayout from "../admin/tsx/layout/AdminLayout";
import TongQuanAdmin from "../admin/tsx/dashboard/TongQuanAdmin";
import DangNhapAdmin from "../admin/tsx/auth/DangNhapAdmin";
import QuenMatKhauAdmin from "../admin/tsx/auth/QuenMatKhauAdmin";

export default function AdminRoutes() {
  return (
    <Routes>
      <Route path="dang-nhap" element={<DangNhapAdmin />} />
      <Route path="quen-mat-khau" element={<QuenMatKhauAdmin />} />
      <Route element={<ProtectedRoute allowedRoles={[ROLES.ADMIN]} />}>
        <Route element={<AdminLayout />}>
          <Route index element={<TongQuanAdmin />} />
        </Route>
      </Route>
    </Routes>
  );
}
