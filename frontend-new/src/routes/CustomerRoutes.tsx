import { Routes, Route } from "react-router-dom";
import GuestRoute from "./GuestRoute";
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
      {/* Auth pages (no layout) */}
      <Route element={<GuestRoute />}>
        <Route path="dang-nhap" element={<DangNhapKhachHang />} />
        <Route path="dang-ky" element={<DangKyKhachHang />} />
      </Route>
      <Route path="quen-mat-khau" element={<QuenMatKhauKhachHang />} />

      {/* Pages with layout */}
      <Route element={<CustomerLayout />}>
        <Route index element={<TrangChu />} />
        <Route element={<ProtectedRoute allowedRoles={[ROLES.CUSTOMER]} />}>
          {/* Protected routes thêm sau */}
        </Route>
      </Route>
    </Routes>
  );
}
