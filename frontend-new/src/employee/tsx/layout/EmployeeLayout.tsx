import { Outlet } from "react-router-dom";

export default function EmployeeLayout() {
  return (
    <div className="employee-layout">
      <Outlet />
    </div>
  );
}
