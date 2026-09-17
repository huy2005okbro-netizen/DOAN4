import { Routes, Route } from "react-router-dom";
import ProtectedRoute from "./ProtectedRoute";
import { ROLES } from "../shared/ts/constants/roles";
import DangNhapKyThuatVien from "../therapist/tsx/auth/DangNhapKyThuatVien";
import QuenMatKhauKyThuatVien from "../therapist/tsx/auth/QuenMatKhauKyThuatVien";
import TongQuanKyThuatVien from "../therapist/tsx/dashboard/TongQuanKyThuatVien";
import LichLamViecCuaToi from "../therapist/tsx/lich-lam-viec/LichLamViecCuaToi";
import ChiTietLichMassage from "../therapist/tsx/lich-massage/ChiTietLichMassage";
import DangThucHienDichVu from "../therapist/tsx/lich-massage/DangThucHienDichVu";
import HoanThanhDichVu from "../therapist/tsx/lich-massage/HoanThanhDichVu";
import KhachHangCuaToi from "../therapist/tsx/khach-hang/KhachHangCuaToi";
import LichSuMassage from "../therapist/tsx/lich-su/LichSuMassage";
import ThongTinCaNhan from "../therapist/tsx/ca-nhan/ThongTinCaNhan";
import DoiMatKhau from "../therapist/tsx/ca-nhan/DoiMatKhau";

export default function TherapistRoutes() {
  return (
    <Routes>
      <Route path="dang-nhap" element={<DangNhapKyThuatVien />} />
      <Route path="quen-mat-khau" element={<QuenMatKhauKyThuatVien />} />
      <Route
        element={
          <ProtectedRoute
            allowedRoles={[ROLES.EMPLOYEE]}
            redirectTo="/therapist/dang-nhap"
          />
        }
      >
        <Route index element={<TongQuanKyThuatVien />} />
        <Route path="lich-lam-viec" element={<LichLamViecCuaToi />} />
        <Route path="lich-massage/:id" element={<ChiTietLichMassage />} />
        <Route
          path="lich-massage/:id/thuc-hien"
          element={<DangThucHienDichVu />}
        />
        <Route
          path="lich-massage/:id/hoan-thanh"
          element={<HoanThanhDichVu />}
        />
        <Route path="khach-hang" element={<KhachHangCuaToi />} />
        <Route path="lich-su" element={<LichSuMassage />} />
        <Route path="ca-nhan" element={<ThongTinCaNhan />} />
        <Route path="ca-nhan/doi-mat-khau" element={<DoiMatKhau />} />
      </Route>
    </Routes>
  );
}
