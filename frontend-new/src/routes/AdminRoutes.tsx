import { Routes, Route, Navigate } from "react-router-dom";
import ProtectedRoute from "./ProtectedRoute";
import { ROLES } from "../shared/ts/constants/roles";
import AdminLayout from "../admin/tsx/layout/AdminLayout";
import TongQuanAdmin from "../admin/tsx/dashboard/TongQuanAdmin";
import DangNhapAdmin from "../admin/tsx/auth/DangNhapAdmin";
import QuenMatKhauAdmin from "../admin/tsx/auth/QuenMatKhauAdmin";
import DanhSachTaiKhoan from "../admin/tsx/tai-khoan/DanhSachTaiKhoan";
import DanhSachKhachHang from "../admin/tsx/khach-hang/DanhSachKhachHang";
import LichSuDatLich from "../admin/tsx/khach-hang/LichSuDatLich";
import LichSuMuaHang from "../admin/tsx/khach-hang/LichSuMuaHang";
import DanhSachNhanVien from "../admin/tsx/nhan-vien/DanhSachNhanVien";
import PhanCaLamViec from "../admin/tsx/nhan-vien/PhanCaLamViec";
import LichSuCongTac from "../admin/tsx/nhan-vien/LichSuCongTac";
import QuanLyChamCong from "../admin/tsx/nhan-vien/QuanLyChamCong";

export default function AdminRoutes() {
  return (
    <Routes>
      <Route path="dang-nhap" element={<DangNhapAdmin />} />
      <Route path="quen-mat-khau" element={<QuenMatKhauAdmin />} />
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
          <Route path="tai-khoan" element={<DanhSachTaiKhoan />} />
          <Route path="khach-hang" element={<DanhSachKhachHang />} />
          <Route path="khach-hang/lich-su-dat-lich" element={<LichSuDatLich />} />
          <Route path="khach-hang/lich-su-mua-hang" element={<LichSuMuaHang />} />
          <Route path="nhan-vien" element={<DanhSachNhanVien />} />
          <Route path="nhan-vien/phan-ca" element={<PhanCaLamViec />} />
          <Route path="nhan-vien/lich-su-cong-tac" element={<LichSuCongTac />} />
          <Route path="nhan-vien/cham-cong" element={<QuanLyChamCong />} />
        </Route>
      </Route>
      <Route path="*" element={<Navigate to="/admin/dang-nhap" replace />} />
    </Routes>
  );
}
