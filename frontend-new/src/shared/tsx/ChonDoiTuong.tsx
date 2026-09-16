import { useNavigate } from "react-router-dom";
import "../css/chon-doi-tuong.css";

export default function ChonDoiTuong() {
  const navigate = useNavigate();

  return (
    <div className="cdt-page">
      <div className="cdt-bg" />
      <div className="cdt-bg-text">SPA</div>

      <div className="cdt-lang">
        <span className="active">VI</span>
        <span className="cdt-lang-divider">|</span>
        <span>EN</span>
      </div>

      <div className="cdt-wrapper">
        {/* Logo */}
        <div className="cdt-logo">
          <svg className="cdt-logo-icon" viewBox="0 0 64 64" fill="none">
            <circle cx="32" cy="32" r="32" fill="rgba(255,255,255,0.12)" />
            {/* Lotus flower */}
            <g transform="translate(32,34)">
              {/* Center petal */}
              <ellipse
                cx="0"
                cy="-10"
                rx="5"
                ry="11"
                fill="#C9A84C"
                opacity="0.95"
                transform="rotate(0)"
              />
              {/* Side petals */}
              <ellipse
                cx="0"
                cy="-9"
                rx="4.5"
                ry="10"
                fill="#E8C86A"
                opacity="0.85"
                transform="rotate(30)"
              />
              <ellipse
                cx="0"
                cy="-9"
                rx="4.5"
                ry="10"
                fill="#E8C86A"
                opacity="0.85"
                transform="rotate(-30)"
              />
              <ellipse
                cx="0"
                cy="-9"
                rx="4"
                ry="9"
                fill="#D4AF52"
                opacity="0.75"
                transform="rotate(60)"
              />
              <ellipse
                cx="0"
                cy="-9"
                rx="4"
                ry="9"
                fill="#D4AF52"
                opacity="0.75"
                transform="rotate(-60)"
              />
              {/* Outer petals */}
              <ellipse
                cx="0"
                cy="-8"
                rx="3.5"
                ry="8"
                fill="#C9A84C"
                opacity="0.6"
                transform="rotate(90)"
              />
              <ellipse
                cx="0"
                cy="-8"
                rx="3.5"
                ry="8"
                fill="#C9A84C"
                opacity="0.6"
                transform="rotate(-90)"
              />
              {/* Stem */}
              <rect
                x="-1.5"
                y="0"
                width="3"
                height="8"
                rx="1.5"
                fill="#C9A84C"
                opacity="0.8"
              />
              {/* Leaf left */}
              <ellipse
                cx="-7"
                cy="4"
                rx="7"
                ry="3"
                fill="#C9A84C"
                opacity="0.7"
                transform="rotate(-20,-7,4)"
              />
              {/* Leaf right */}
              <ellipse
                cx="7"
                cy="4"
                rx="7"
                ry="3"
                fill="#C9A84C"
                opacity="0.7"
                transform="rotate(20,7,4)"
              />
            </g>
          </svg>
          <span className="cdt-logo-name">MASSAGE SPA</span>
          <span className="cdt-logo-tagline">
            Thư giãn · Tái tạo năng lượng
          </span>
        </div>

        {/* Hero */}
        <div className="cdt-hero">
          <p className="cdt-hero-title">Chào mừng bạn đến với</p>
          <h1 className="cdt-hero-main">MASSAGE SPA</h1>
          <p className="cdt-hero-sub">
            Đăng nhập để trải nghiệm dịch vụ
            <br />
            chuyên nghiệp và tiện lợi
          </p>
        </div>

        {/* Card */}
        <div className="cdt-card">
          <p className="cdt-card-title">Bạn là ai?</p>
          <p className="cdt-card-sub">Chọn đối tượng để đăng nhập</p>

          <div className="cdt-roles">
            <button
              className="cdt-role-btn cdt-role-btn--admin"
              onClick={() => navigate("/admin/dang-nhap")}
            >
              <div className="cdt-role-icon">
                <svg width="22" height="22" viewBox="0 0 24 24" fill="none">
                  <path
                    d="M12 2L15.09 8.26L22 9.27L17 14.14L18.18 21.02L12 17.77L5.82 21.02L7 14.14L2 9.27L8.91 8.26L12 2Z"
                    fill="#fff"
                  />
                </svg>
              </div>
              <div className="cdt-role-info">
                <span className="cdt-role-name">Quản trị viên (Admin)</span>
                <span className="cdt-role-desc">Quản lý toàn bộ hệ thống</span>
              </div>
            </button>

            <button
              className="cdt-role-btn cdt-role-btn--employee"
              onClick={() => navigate("/employee/dang-nhap")}
            >
              <div className="cdt-role-icon">
                <svg width="22" height="22" viewBox="0 0 24 24" fill="none">
                  <circle cx="12" cy="8" r="4" fill="#fff" />
                  <path
                    d="M4 20c0-4 3.58-7 8-7s8 3 8 7"
                    stroke="#fff"
                    strokeWidth="2"
                    strokeLinecap="round"
                  />
                  <circle cx="18" cy="8" r="2.5" fill="rgba(255,255,255,0.7)" />
                  <circle cx="6" cy="8" r="2.5" fill="rgba(255,255,255,0.7)" />
                </svg>
              </div>
              <div className="cdt-role-info">
                <span className="cdt-role-name">Nhân viên</span>
                <span className="cdt-role-desc">
                  Phục vụ và quản lý dịch vụ
                </span>
              </div>
            </button>

            <button
              className="cdt-role-btn cdt-role-btn--therapist"
              onClick={() => navigate("/therapist/dang-nhap")}
            >
              <div className="cdt-role-icon">
                <svg width="22" height="22" viewBox="0 0 24 24" fill="none">
                  <circle cx="10" cy="7" r="3.5" fill="#fff" />
                  <path
                    d="M3 20v-2a5 5 0 0 1 5-5h4a5 5 0 0 1 5 5v2"
                    stroke="#fff"
                    strokeWidth="2"
                    strokeLinecap="round"
                  />
                  <path
                    d="M16 9l1.5 1.5L21 7"
                    stroke="#fff"
                    strokeWidth="2"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                  />
                </svg>
              </div>
              <div className="cdt-role-info">
                <span className="cdt-role-name">Kỹ thuật viên</span>
                <span className="cdt-role-desc">
                  Thực hiện liệu trình massage
                </span>
              </div>
            </button>

            <button
              className="cdt-role-btn cdt-role-btn--customer"
              onClick={() => navigate("/customer/dang-nhap")}
            >
              <div className="cdt-role-icon">
                <svg width="22" height="22" viewBox="0 0 24 24" fill="none">
                  <circle cx="12" cy="8" r="4" fill="#fff" />
                  <path
                    d="M4 20c0-4 3.58-7 8-7s8 3 8 7"
                    stroke="#fff"
                    strokeWidth="2"
                    strokeLinecap="round"
                  />
                </svg>
              </div>
              <div className="cdt-role-info">
                <span className="cdt-role-name">Khách hàng</span>
                <span className="cdt-role-desc">
                  Đặt lịch, mua sản phẩm, trải nghiệm dịch vụ
                </span>
              </div>
            </button>
          </div>
        </div>

        <p className="cdt-footer">Trải nghiệm thư giãn · Sức khoẻ · Sắc đẹp</p>
      </div>
    </div>
  );
}
