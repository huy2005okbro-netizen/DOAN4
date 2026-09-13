import { Routes, Route } from "react-router-dom";
import GuestRoute from "./GuestRoute";
import ProtectedRoute from "./ProtectedRoute";
import { ROLES } from "../shared/ts/constants/roles";
import CustomerLayout from "../customer/tsx/layout/CustomerLayout";
import TrangChu from "../customer/tsx/home/TrangChu";
import DangNhapKhachHang from "../customer/tsx/auth/DangNhapKhachHang";
import DangKyKhachHang from "../customer/tsx/auth/DangKyKhachHang";

export default function CustomerRoutes() {
  return (
    <Routes>
      <Route element={<CustomerLayout />}>
        <Route index element={<TrangChu />} />

        {/* Guest only */}
        <Route element={<GuestRoute />}>
          <Route path="dang-nhap" element={<DangNhapKhachHang />} />
          <Route path="dang-ky" element={<DangKyKhachHang />} />
        </Route>

        {/* Protected customer routes */}
        <Route element={<ProtectedRoute allowedRoles={[ROLES.CUSTOMER]} />}>
          {/* Thêm các route khác tại đây */}
        </Route>
      </Route>
    </Routes>
  );
}
