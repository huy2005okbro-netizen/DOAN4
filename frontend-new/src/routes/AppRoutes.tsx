import { Routes, Route } from "react-router-dom";
import AdminRoutes from "./AdminRoutes";
import EmployeeRoutes from "./EmployeeRoutes";
import CustomerRoutes from "./CustomerRoutes";
import TherapistRoutes from "./TherapistRoutes";
import ChonDoiTuong from "../shared/tsx/ChonDoiTuong";
import NotFound from "../shared/tsx/errors/NotFound";

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<ChonDoiTuong />} />
      <Route path="/admin/*" element={<AdminRoutes />} />
      <Route path="/employee/*" element={<EmployeeRoutes />} />
      <Route path="/therapist/*" element={<TherapistRoutes />} />
      <Route path="/customer/*" element={<CustomerRoutes />} />
      <Route path="*" element={<NotFound />} />
    </Routes>
  );
}
