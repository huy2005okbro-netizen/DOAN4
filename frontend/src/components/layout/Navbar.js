import React from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import "./Navbar.css";

const Navbar = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <nav className="navbar">
      <div className="navbar-brand">
        <Link to="/">Đồ Án 4</Link>
      </div>
      <ul className="navbar-links">
        <li>
          <Link to="/">Trang Chủ</Link>
        </li>
        {user ? (
          <>
            <li>
              <Link to="/dashboard">Dashboard</Link>
            </li>
            <li>
              <Link to="/profile">Hồ Sơ</Link>
            </li>
            <li>
              <button className="btn-logout" onClick={handleLogout}>
                Đăng Xuất
              </button>
            </li>
          </>
        ) : (
          <>
            <li>
              <Link to="/login">Đăng Nhập</Link>
            </li>
            <li>
              <Link to="/register">Đăng Ký</Link>
            </li>
          </>
        )}
      </ul>
    </nav>
  );
};

export default Navbar;
