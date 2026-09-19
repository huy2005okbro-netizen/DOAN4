import { Routes, Route } from "react-router-dom";
import ProtectedRoute from "./ProtectedRoute";
import { ROLES } from "../shared/ts/constants/roles";
import CustomerLayout from "../customer/tsx/layout/CustomerLayout";
import TrangChu from "../customer/tsx/home/TrangChu";
import DangNhapKhachHang from "../customer/tsx/auth/DangNhapKhachHang";
import DangKyKhachHang from "../customer/tsx/auth/DangKyKhachHang";
import QuenMatKhauKhachHang from "../customer/tsx/auth/QuenMatKhauKhachHang";

export default function CustomerRoutes() {
  return (
    <Routes>
      {/* Auth pages — không cần layout */}
      <Route path="dang-nhap" element={<DangNhapKhachHang />} />
      <Route path="dang-ky" element={<DangKyKhachHang />} />
      <Route path="quen-mat-khau" element={<QuenMatKhauKhachHang />} />

      <Route element={<ProtectedRoute allowedRoles={[ROLES.CUSTOMER]} redirectTo="/customer/dang-nhap" />}>
        <Route element={<CustomerLayout />}><Route index element={<TrangChu />} /></Route>
      </Route>
    </Routes>
  );
}
