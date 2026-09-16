import { Routes, Route } from "react-router-dom";
import ProtectedRoute from "./ProtectedRoute";
import { ROLES } from "../shared/ts/constants/roles";
import EmployeeLayout from "../employee/tsx/layout/EmployeeLayout";
import TongQuanNhanVien from "../employee/tsx/dashboard/TongQuanNhanVien";
import DangNhapNhanVien from "../employee/tsx/auth/DangNhapNhanVien";
import QuenMatKhauNhanVien from "../employee/tsx/auth/QuenMatKhauNhanVien";

export default function EmployeeRoutes() {
  return (
    <Routes>
      <Route path="dang-nhap" element={<DangNhapNhanVien />} />
      <Route path="quen-mat-khau" element={<QuenMatKhauNhanVien />} />
      <Route element={<ProtectedRoute allowedRoles={[ROLES.EMPLOYEE]} />}>
        <Route element={<EmployeeLayout />}>
          <Route index element={<TongQuanNhanVien />} />
        </Route>
      </Route>
    </Routes>
  );
}
