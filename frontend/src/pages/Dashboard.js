import React from "react";
import { useAuth } from "../context/AuthContext";
import "./Dashboard.css";

const Dashboard = () => {
  const { user } = useAuth();

  return (
    <div className="dashboard-container">
      <div className="dashboard-header">
        <h1>Dashboard</h1>
        <p>
          Xin chào, <strong>{user?.name}</strong>!
        </p>
      </div>
      <div className="dashboard-cards">
        <div className="card">
          <h3>Thông tin tài khoản</h3>
          <p>
            <span>Email:</span> {user?.email}
          </p>
          <p>
            <span>Vai trò:</span> {user?.role}
          </p>
        </div>
        <div className="card">
          <h3>Thống kê</h3>
          <p>Chào mừng bạn đến với hệ thống</p>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
