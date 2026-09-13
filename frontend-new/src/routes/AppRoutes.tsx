import { Routes, Route, Navigate } from "react-router-dom";
import AdminRoutes from "./AdminRoutes";
import EmployeeRoutes from "./EmployeeRoutes";
import CustomerRoutes from "./CustomerRoutes";
import NotFound from "../shared/tsx/errors/NotFound";

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<Navigate to="/customer" replace />} />
      <Route path="/admin/*" element={<AdminRoutes />} />
      <Route path="/employee/*" element={<EmployeeRoutes />} />
      <Route path="/customer/*" element={<CustomerRoutes />} />
      <Route path="*" element={<NotFound />} />
    </Routes>
  );
}
