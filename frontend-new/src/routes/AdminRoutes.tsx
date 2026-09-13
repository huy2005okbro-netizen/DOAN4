import { Routes, Route } from "react-router-dom";
import ProtectedRoute from "./ProtectedRoute";
import { ROLES } from "../shared/ts/constants/roles";
import AdminLayout from "../admin/tsx/layout/AdminLayout";
import TongQuanAdmin from "../admin/tsx/dashboard/TongQuanAdmin";
import DangNhapAdmin from "../admin/tsx/auth/DangNhapAdmin";

export default function AdminRoutes() {
  return (
    <Routes>
      <Route path="dang-nhap" element={<DangNhapAdmin />} />
      <Route element={<ProtectedRoute allowedRoles={[ROLES.ADMIN]} />}>
        <Route element={<AdminLayout />}>
          <Route index element={<TongQuanAdmin />} />
          {/* Thêm các route khác tại đây */}
        </Route>
      </Route>
    </Routes>
  );
}
