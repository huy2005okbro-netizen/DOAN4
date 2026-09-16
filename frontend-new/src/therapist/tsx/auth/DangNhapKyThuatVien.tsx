import { useState, FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../../shared/ts/hooks/useAuth";
import api from "../../../shared/ts/api/api";
import type { AuthResponse, LoginRequest } from "../../../shared/ts/types/auth";
import "../../css/auth.css";

/* ===== Logo hoa sen vàng (SVG inline) ===== */
const LotusLogo = () => (
  <svg
    className="auth-logo-icon"
    viewBox="0 0 80 80"
    fill="none"
    xmlns="http://www.w3.org/2000/svg"
  >
    {/* Cánh trung tâm */}
    <ellipse cx="40" cy="28" rx="6.5" ry="15" fill="#E8C86A" />
    {/* Cánh trái gần */}
    <ellipse
      cx="40"
      cy="30"
      rx="5.5"
      ry="13"
      fill="#D4AF52"
      transform="rotate(-28 40 30)"
    />
    {/* Cánh phải gần */}
    <ellipse
      cx="40"
      cy="30"
      rx="5.5"
      ry="13"
      fill="#D4AF52"
      transform="rotate(28 40 30)"
    />
    {/* Cánh trái xa */}
    <ellipse
      cx="40"
      cy="32"
      rx="5"
      ry="11"
      fill="#C9A84C"
      opacity="0.85"
      transform="rotate(-56 40 32)"
    />
    {/* Cánh phải xa */}
    <ellipse
      cx="40"
      cy="32"
      rx="5"
      ry="11"
      fill="#C9A84C"
      opacity="0.85"
      transform="rotate(56 40 32)"
    />
    {/* Cánh ngang trái */}
    <ellipse
      cx="40"
      cy="34"
      rx="4.5"
      ry="9"
      fill="#B8963E"
      opacity="0.7"
      transform="rotate(-80 40 34)"
    />
    {/* Cánh ngang phải */}
    <ellipse
      cx="40"
      cy="34"
      rx="4.5"
      ry="9"
      fill="#B8963E"
      opacity="0.7"
      transform="rotate(80 40 34)"
    />
    {/* Cuống */}
    <rect
      x="38.5"
      y="43"
      width="3"
      height="10"
      rx="1.5"
      fill="#C9A84C"
      opacity="0.85"
    />
    {/* Lá trái */}
    <ellipse
      cx="30"
      cy="52"
      rx="10"
      ry="3.5"
      fill="#C9A84C"
      opacity="0.75"
      transform="rotate(-25 30 52)"
    />
    {/* Lá phải */}
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

/* ===== Avatar icon: bàn tay massage ===== */
const TherapistIcon = () => (
  <svg width="36" height="36" viewBox="0 0 24 24" fill="none">
    {/* Thân người + tay */}
    <circle cx="12" cy="5.5" r="3" fill="#fff" />
    <path
      d="M7 21v-3a5 5 0 0 1 5-5h0a5 5 0 0 1 5 5v3"
      stroke="#fff"
      strokeWidth="1.8"
      strokeLinecap="round"
    />
    {/* Dấu tích nhỏ — biểu thị kỹ thuật viên */}
    <path
      d="M15.5 9.5l1.5 1.5 2.5-2.5"
      stroke="#fff"
      strokeWidth="1.8"
      strokeLinecap="round"
      strokeLinejoin="round"
    />
  </svg>
);

export default function DangNhapKyThuatVien() {
  const navigate = useNavigate();
  const { login } = useAuth();

  const [form, setForm] = useState<LoginRequest>({ email: "", password: "" });
  const [showPwd, setShowPwd] = useState(false);
  const [remember, setRemember] = useState(true);
  const [errors, setErrors] = useState<Partial<LoginRequest>>({});
  const [serverError, setServerError] = useState("");
  const [loading, setLoading] = useState(false);

  const validate = () => {
    const e: Partial<LoginRequest> = {};
    if (!form.email.trim()) e.email = "Mã nhân viên hoặc email là bắt buộc";
    if (!form.password) e.password = "Mật khẩu là bắt buộc";
    setErrors(e);
    return Object.keys(e).length === 0;
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setServerError("");
    if (!validate()) return;
    setLoading(true);
    try {
      const { data } = await api.post<AuthResponse>("/auth/login", form);
      if (data.role !== "EMPLOYEE") {
        setServerError("Tài khoản không có quyền Kỹ thuật viên");
        return;
      }
      login(data.token, {
        id: data.userId,
        fullName: data.fullName,
        email: data.email,
        role: data.role,
      });
      navigate("/therapist", { replace: true });
    } catch (err: unknown) {
      const e = err as { response?: { data?: { message?: string } } };
      setServerError(
        e.response?.data?.message || "Đăng nhập thất bại. Vui lòng thử lại.",
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-page">
      {/* Background spa với nến và giường massage */}
      <div className="auth-bg" />

      <div className="auth-container">
        {/* Nút quay lại */}
        <button className="auth-back" onClick={() => navigate("/")}>
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
          Quay lại chọn đối tượng
        </button>

        {/* Logo */}
        <div className="auth-logo">
          <LotusLogo />
          <span className="auth-logo-name">MASSAGE SPA</span>
          <span className="auth-logo-tagline">
            Thư giãn · Tái tạo năng lượng
          </span>
        </div>

        {/* Card */}
        <div className="auth-card">
          {/* Avatar */}
          <div className="auth-avatar">
            <TherapistIcon />
          </div>

          <h1 className="auth-title">Đăng nhập Kỹ thuật viên</h1>
          <p className="auth-subtitle">
            Truy cập hệ thống dành cho kỹ thuật viên massage
          </p>

          <form className="auth-form" onSubmit={handleSubmit} noValidate>
            {serverError && (
              <div className="auth-server-error">{serverError}</div>
            )}

            {/* Mã nhân viên / Email */}
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
                    <path
                      d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"
                      strokeLinecap="round"
                    />
                    <circle cx="12" cy="7" r="4" />
                  </svg>
                </span>
                <input
                  type="text"
                  className={`auth-input${errors.email ? " error" : ""}`}
                  placeholder="Mã nhân viên / Email"
                  value={form.email}
                  onChange={(e) =>
                    setForm((f) => ({ ...f, email: e.target.value }))
                  }
                  autoComplete="username"
                  autoFocus
                />
              </div>
              {errors.email && (
                <span className="auth-error-text">{errors.email}</span>
              )}
            </div>

            {/* Mật khẩu */}
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
                    <rect x="3" y="11" width="18" height="11" rx="2" ry="2" />
                    <path d="M7 11V7a5 5 0 0 1 10 0v4" strokeLinecap="round" />
                  </svg>
                </span>
                <input
                  type={showPwd ? "text" : "password"}
                  className={`auth-input${errors.password ? " error" : ""}`}
                  placeholder="Mật khẩu"
                  value={form.password}
                  onChange={(e) =>
                    setForm((f) => ({ ...f, password: e.target.value }))
                  }
                  autoComplete="current-password"
                />
                <button
                  type="button"
                  className="auth-eye"
                  onClick={() => setShowPwd((s) => !s)}
                  aria-label={showPwd ? "Ẩn mật khẩu" : "Hiện mật khẩu"}
                >
                  {showPwd ? (
                    <svg
                      width="18"
                      height="18"
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                      strokeWidth="2"
                    >
                      <path
                        d="M17.94 17.94A10.07 10.07 0 0112 20c-7 0-11-8-11-8a18.45 18.45 0 015.06-5.94"
                        strokeLinecap="round"
                      />
                      <path
                        d="M9.9 4.24A9.12 9.12 0 0112 4c7 0 11 8 11 8a18.5 18.5 0 01-2.16 3.19"
                        strokeLinecap="round"
                      />
                      <path
                        d="M14.12 14.12a3 3 0 11-4.24-4.24"
                        strokeLinecap="round"
                      />
                      <line
                        x1="1"
                        y1="1"
                        x2="23"
                        y2="23"
                        strokeLinecap="round"
                      />
                    </svg>
                  ) : (
                    <svg
                      width="18"
                      height="18"
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                      strokeWidth="2"
                    >
                      <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" />
                      <circle cx="12" cy="12" r="3" />
                    </svg>
                  )}
                </button>
              </div>
              {errors.password && (
                <span className="auth-error-text">{errors.password}</span>
              )}
            </div>

            {/* Ghi nhớ + Quên mật khẩu */}
            <div className="auth-row">
              <label className="auth-remember">
                <input
                  type="checkbox"
                  checked={remember}
                  onChange={(e) => setRemember(e.target.checked)}
                />
                Ghi nhớ đăng nhập
              </label>
              <button
                type="button"
                className="auth-forgot"
                onClick={() => navigate("/therapist/quen-mat-khau")}
              >
                Quên mật khẩu?
              </button>
            </div>

            {/* Nút đăng nhập */}
            <button type="submit" className="auth-submit" disabled={loading}>
              {loading ? (
                <span className="auth-spinner" />
              ) : (
                <>
                  Đăng nhập
                  <svg
                    width="18"
                    height="18"
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    strokeWidth="2.5"
                  >
                    <path
                      d="M5 12h14M12 5l7 7-7 7"
                      strokeLinecap="round"
                      strokeLinejoin="round"
                    />
                  </svg>
                </>
              )}
            </button>
          </form>
        </div>

        {/* Quote phía dưới */}
        <p className="auth-quote">
          "Bàn tay của bạn
          <br />
          mang lại sức khoẻ và hạnh phúc"
        </p>
      </div>
    </div>
  );
}
