import { useState, FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import api from "../../../shared/ts/api/api";
import "../../css/auth.css";

const LotusLogo = () => (
  <svg className="auth-logo-icon" viewBox="0 0 80 80" fill="none">
    <ellipse cx="40" cy="28" rx="6.5" ry="15" fill="#E8C86A" />
    <ellipse
      cx="40"
      cy="30"
      rx="5.5"
      ry="13"
      fill="#D4AF52"
      transform="rotate(-28 40 30)"
    />
    <ellipse
      cx="40"
      cy="30"
      rx="5.5"
      ry="13"
      fill="#D4AF52"
      transform="rotate(28 40 30)"
    />
    <ellipse
      cx="40"
      cy="32"
      rx="5"
      ry="11"
      fill="#C9A84C"
      opacity="0.85"
      transform="rotate(-56 40 32)"
    />
    <ellipse
      cx="40"
      cy="32"
      rx="5"
      ry="11"
      fill="#C9A84C"
      opacity="0.85"
      transform="rotate(56 40 32)"
    />
    <rect
      x="38.5"
      y="43"
      width="3"
      height="10"
      rx="1.5"
      fill="#C9A84C"
      opacity="0.85"
    />
    <ellipse
      cx="30"
      cy="52"
      rx="10"
      ry="3.5"
      fill="#C9A84C"
      opacity="0.75"
      transform="rotate(-25 30 52)"
    />
    <ellipse
      cx="50"
      cy="52"
      rx="10"
      ry="3.5"
      fill="#C9A84C"
      opacity="0.75"
      transform="rotate(25 50 52)"
    />
  </svg>
);

export default function QuenMatKhauKyThuatVien() {
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
    if (!/\S+@\S+\.\S+/.test(email)) {
      setError("Email không hợp lệ");
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
      <div className="auth-bg" />
      <div className="auth-container">
        <button
          className="auth-back"
          onClick={() => navigate("/therapist/dang-nhap")}
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

        <div className="auth-logo">
          <LotusLogo />
          <span className="auth-logo-name">MASSAGE SPA</span>
          <span className="auth-logo-tagline">
            Thư giãn · Tái tạo năng lượng
          </span>
        </div>

        <div className="auth-card">
          <div className="auth-avatar">
            <svg
              width="34"
              height="34"
              viewBox="0 0 24 24"
              fill="none"
              stroke="#fff"
              strokeWidth="2"
            >
              <circle cx="12" cy="12" r="10" />
              <line x1="12" y1="8" x2="12" y2="12" strokeLinecap="round" />
              <line x1="12" y1="16" x2="12.01" y2="16" strokeLinecap="round" />
            </svg>
          </div>
          <h1 className="auth-title">Quên mật khẩu</h1>
          <p className="auth-subtitle">
            Nhập email để nhận hướng dẫn đặt lại mật khẩu
          </p>

          {success ? (
            <div style={{ textAlign: "center", padding: "16px 0" }}>
              <div style={{ fontSize: 52, marginBottom: 14 }}>✅</div>
              <p style={{ fontSize: 14, color: "#374151", lineHeight: 1.7 }}>
                Email hướng dẫn đã được gửi tới
                <br />
                <strong>{email}</strong>
                <br />
                Vui lòng kiểm tra hộp thư.
              </p>
              <button
                style={{
                  marginTop: 20,
                  background: "none",
                  border: "none",
                  color: "#16A34A",
                  fontWeight: 700,
                  cursor: "pointer",
                  fontSize: 14,
                }}
                onClick={() => navigate("/therapist/dang-nhap")}
              >
                ← Quay lại đăng nhập
              </button>
            </div>
          ) : (
            <form className="auth-form" onSubmit={handleSubmit} noValidate>
              {error && <div className="auth-server-error">{error}</div>}
              <div className="auth-field">
                <div className="auth-input-wrap">
                  <span className="auth-input-icon">
                    <svg
                      width="17"
                      height="17"
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
                    autoFocus
                  />
                </div>
              </div>
              <button type="submit" className="auth-submit" disabled={loading}>
                {loading ? <span className="auth-spinner" /> : "Gửi hướng dẫn"}
              </button>
            </form>
          )}
        </div>

        <p className="auth-quote">
          "Bàn tay của bạn
          <br />
          mang lại sức khoẻ và hạnh phúc"
        </p>
      </div>
    </div>
  );
}
