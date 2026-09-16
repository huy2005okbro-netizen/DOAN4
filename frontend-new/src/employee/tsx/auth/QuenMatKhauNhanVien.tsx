import { useState, FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import api from "../../../shared/ts/api/api";
import "../../css/auth.css";

const LotusIcon = () => (
  <svg width="28" height="28" viewBox="0 0 64 64" fill="none">
    <g transform="translate(32,36)">
      <ellipse cx="0" cy="-10" rx="5" ry="11" fill="#C9A84C" opacity="0.95" />
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
      <rect
        x="-1.5"
        y="0"
        width="3"
        height="7"
        rx="1.5"
        fill="#C9A84C"
        opacity="0.8"
      />
      <ellipse
        cx="-6"
        cy="4"
        rx="6"
        ry="2.5"
        fill="#C9A84C"
        opacity="0.7"
        transform="rotate(-20,-6,4)"
      />
      <ellipse
        cx="6"
        cy="4"
        rx="6"
        ry="2.5"
        fill="#C9A84C"
        opacity="0.7"
        transform="rotate(20,6,4)"
      />
    </g>
  </svg>
);

export default function QuenMatKhauNhanVien() {
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState(false);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError("");
    if (!email.trim()) {
      setError("Email là bắt buộc");
      return;
    }
    setLoading(true);
    try {
      await api.post("/auth/forgot-password", { email });
      setSuccess(true);
    } catch {
      setError("Không tìm thấy tài khoản với email này.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-page">
      <div
        className="auth-bg"
        style={{
          background:
            "linear-gradient(135deg, rgba(10,30,15,0.85) 0%, rgba(20,50,30,0.75) 50%, rgba(40,80,50,0.6) 100%), url(https://images.unsplash.com/photo-1571019614242-c5c5dee9f50b?w=1400&q=80) center/cover no-repeat",
        }}
      />
      <div className="auth-container">
        <div style={{ position: "relative", width: "100%", maxWidth: 400 }}>
          <button
            className="auth-back"
            onClick={() => navigate("/employee/dang-nhap")}
          >
            <svg
              width="16"
              height="16"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2.5"
            >
              <path
                d="M19 12H5M12 5l-7 7 7 7"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
            </svg>
            Quay lại đăng nhập
          </button>

          <div className="auth-card">
            <div className="auth-logo-mini">
              <LotusIcon />
              <span className="auth-logo-mini-name">MASSAGE SPA</span>
            </div>

            <div className="auth-header">
              <div className="auth-avatar auth-avatar--employee">
                <svg
                  width="30"
                  height="30"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="#fff"
                  strokeWidth="2"
                >
                  <circle cx="12" cy="12" r="10" />
                  <line x1="12" y1="8" x2="12" y2="12" strokeLinecap="round" />
                  <line
                    x1="12"
                    y1="16"
                    x2="12.01"
                    y2="16"
                    strokeLinecap="round"
                  />
                </svg>
              </div>
              <h1 className="auth-title">Quên mật khẩu</h1>
              <p className="auth-subtitle">
                Nhập email để nhận hướng dẫn đặt lại mật khẩu
              </p>
            </div>

            {success ? (
              <div style={{ textAlign: "center", padding: "16px 0" }}>
                <div style={{ fontSize: 48, marginBottom: 12 }}>✅</div>
                <p style={{ fontSize: 14, color: "#374151", lineHeight: 1.6 }}>
                  Email hướng dẫn đã được gửi tới <strong>{email}</strong>.
                  <br />
                  Vui lòng kiểm tra hộp thư của bạn.
                </p>
                <button
                  style={{
                    marginTop: 20,
                    background: "none",
                    border: "none",
                    color: "#16A34A",
                    fontWeight: 600,
                    cursor: "pointer",
                    fontSize: 14,
                  }}
                  onClick={() => navigate("/employee/dang-nhap")}
                >
                  Quay lại đăng nhập
                </button>
              </div>
            ) : (
              <form className="auth-form" onSubmit={handleSubmit} noValidate>
                {error && <div className="auth-server-error">{error}</div>}
                <div className="auth-field">
                  <label className="auth-label">Email</label>
                  <div className="auth-input-wrap">
                    <span className="auth-input-icon">
                      <svg
                        width="16"
                        height="16"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        strokeWidth="2"
                      >
                        <path d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z" />
                        <polyline points="22,6 12,13 2,6" />
                      </svg>
                    </span>
                    <input
                      type="email"
                      className="auth-input"
                      placeholder="Nhập email của bạn"
                      value={email}
                      onChange={(e) => setEmail(e.target.value)}
                      autoComplete="email"
                    />
                  </div>
                </div>
                <button
                  type="submit"
                  className="auth-submit auth-submit--employee"
                  disabled={loading}
                >
                  {loading ? (
                    <span className="auth-spinner" />
                  ) : (
                    "Gửi hướng dẫn"
                  )}
                </button>
              </form>
            )}
          </div>
        </div>
      </div>
      <div className="auth-footer">
        — MASSAGE SPA &nbsp;|&nbsp; Sức khoẻ hôm nay · Hạnh phúc ngày mai —
      </div>
    </div>
  );
}
