import React from "react";
import { Link } from "react-router-dom";
import "./Home.css";

const Home = () => {
  return (
    <div className="home-container">
      <h1>Chào mừng đến với Đồ Án 4</h1>
      <p>Ứng dụng web được xây dựng với React + Node.js + MongoDB</p>
      <div className="home-buttons">
        <Link to="/register" className="btn btn-primary">
          Đăng Ký
        </Link>
        <Link to="/login" className="btn btn-secondary">
          Đăng Nhập
        </Link>
      </div>
    </div>
  );
};

export default Home;
