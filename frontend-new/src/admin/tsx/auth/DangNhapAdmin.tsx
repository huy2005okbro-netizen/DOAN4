import { useState, FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../../shared/ts/hooks/useAuth";
import api from "../../../shared/ts/api/api";
import type { AuthResponse, LoginRequest } from "../../../shared/ts/types/auth";
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

export default function DangNhapAdmin() {
  const navigate = useNavigate();
  const { login } = useAuth();

  const [form, setForm] = useState<LoginRequest>({ email: "", password: "" });
  const [showPwd, setShowPwd] = useState(false);
  const [remember, setRemember] = useState(false);
  const [errors, setErrors] = useState<Partial<LoginRequest>>({});
  const [serverError, setServerError] = useState("");
  const [loading, setLoading] = useState(false);

  const validate = () => {
    const e: Partial<LoginRequest> = {};
    if (!form.email.trim()) e.email = "Email là bắt buộc";
    else if (!/\S+@\S+\.\S+/.test(form.email)) e.email = "Email không hợp lệ";
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
      console.log("[Login] Response:", data); // debug
      if (data.role !== "ADMIN") {
        setServerError(`Tài khoản có role "${data.role}" không phải Admin`);
        return;
      }
      login(data.token, {
        id: data.userId,
        fullName: data.fullName,
        email: data.email,
        role: data.role,
      });
      // Dùng window.location thay navigate để force full redirect
      window.location.href = "/admin";
    } catch (err: unknown) {
      const e = err as {
        response?: { data?: { message?: string }; status?: number };
      };
      console.error("[Login] Error:", e.response);
      setServerError(
        e.response?.data?.message ||
          `Lỗi ${e.response?.status ?? ""}: Đăng nhập thất bại. Kiểm tra email/mật khẩu.`,
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-page">
      <div className="auth-bg" />
      <div className="auth-container">
        <div style={{ position: "relative", width: "100%", maxWidth: 400 }}>
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

          <div className="auth-card">
            {/* Logo mini */}
            <div className="auth-logo-mini">
              <LotusIcon />
              <span className="auth-logo-mini-name">MASSAGE SPA</span>
            </div>

            {/* Avatar + title */}
            <div className="auth-header">
              <div className="auth-avatar auth-avatar--admin">
                <svg width="32" height="32" viewBox="0 0 24 24" fill="none">
                  <path
                    d="M12 2L15.09 8.26L22 9.27L17 14.14L18.18 21.02L12 17.77L5.82 21.02L7 14.14L2 9.27L8.91 8.26L12 2Z"
                    fill="#fff"
                  />
                </svg>
              </div>
              <h1 className="auth-title">Đăng nhập Admin</h1>
              <p className="auth-subtitle">Truy cập hệ thống quản trị</p>
            </div>

            <form className="auth-form" onSubmit={handleSubmit} noValidate>
              {serverError && (
                <div className="auth-server-error">{serverError}</div>
              )}

              {/* Email */}
              <div className="auth-field">
                <label className="auth-label">Tên đăng nhập / Email</label>
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
                      <path
                        d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"
                        strokeLinecap="round"
                      />
                      <circle cx="12" cy="7" r="4" />
                    </svg>
                  </span>
                  <input
                    type="email"
                    className={`auth-input${errors.email ? " error" : ""}`}
                    placeholder="Nhập tên đăng nhập hoặc email"
                    value={form.email}
                    onChange={(e) =>
                      setForm((f) => ({ ...f, email: e.target.value }))
                    }
                    autoComplete="email"
                  />
                </div>
                {errors.email && (
                  <span className="auth-error-text">{errors.email}</span>
                )}
              </div>

              {/* Password */}
              <div className="auth-field">
                <label className="auth-label">Mật khẩu</label>
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
                      <rect x="3" y="11" width="18" height="11" rx="2" ry="2" />
                      <path
                        d="M7 11V7a5 5 0 0 1 10 0v4"
                        strokeLinecap="round"
                      />
                    </svg>
                  </span>
                  <input
                    type={showPwd ? "text" : "password"}
                    className={`auth-input${errors.password ? " error" : ""}`}
                    placeholder="Nhập mật khẩu"
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
                    aria-label="Toggle password"
                  >
                    {showPwd ? (
                      <svg
                        width="17"
                        height="17"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        strokeWidth="2"
                      >
                        <path
                          d="M17.94 17.94A10.07 10.07 0 0112 20c-7 0-11-8-11-8a18.45 18.45 0 015.06-5.94M9.9 4.24A9.12 9.12 0 0112 4c7 0 11 8 11 8a18.5 18.5 0 01-2.16 3.19m-6.72-1.07a3 3 0 11-4.24-4.24"
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
                        width="17"
                        height="17"
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

              {/* Remember + Forgot */}
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
                  onClick={() => navigate("/admin/quen-mat-khau")}
                >
                  Quên mật khẩu?
                </button>
              </div>

              {/* Submit */}
              <button
                type="submit"
                className="auth-submit auth-submit--admin"
                disabled={loading}
              >
                {loading ? (
                  <span className="auth-spinner" />
                ) : (
                  <>
                    Đăng nhập
                    <svg
                      width="16"
                      height="16"
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

              {/* Divider + Social */}
              <div className="auth-divider">Hoặc đăng nhập với</div>
              <div className="auth-socials">
                <button type="button" className="auth-social-btn">
                  <svg width="18" height="18" viewBox="0 0 48 48">
                    <path
                      fill="#4285F4"
                      d="M44.5 20H24v8.5h11.8C34.7 33.9 29.9 37 24 37c-7.2 0-13-5.8-13-13s5.8-13 13-13c3.1 0 5.9 1.1 8.1 2.9l6.4-6.4C34.6 5.1 29.6 3 24 3 12.4 3 3 12.4 3 24s9.4 21 21 21c10.5 0 20-7.6 20-21 0-1.4-.2-2.7-.5-4z"
                    />
                    <path
                      fill="#34A853"
                      d="M6.3 14.7l7 5.1C15.2 16.5 19.3 14 24 14c3.1 0 5.9 1.1 8.1 2.9l6.4-6.4C34.6 5.1 29.6 3 24 3c-7.7 0-14.4 4.4-17.7 10.7z"
                    />
                    <path
                      fill="#FBBC05"
                      d="M24 45c5.8 0 10.7-1.9 14.3-5.2l-6.6-5.4C29.8 36.1 27 37 24 37c-5.8 0-10.7-3.9-12.5-9.3l-7 5.4C8 40.1 15.4 45 24 45z"
                    />
                    <path
                      fill="#EA4335"
                      d="M44.5 20H24v8.5h11.8c-.9 2.6-2.6 4.7-4.8 6.1l6.6 5.4C41.4 36.5 45 30.9 45 24c0-1.4-.2-2.7-.5-4z"
                    />
                  </svg>
                  Google
                </button>
                <button type="button" className="auth-social-btn">
                  <svg width="18" height="18" viewBox="0 0 23 23">
                    <path fill="#f3f3f3" d="M0 0h23v23H0z" />
                    <path fill="#f35325" d="M1 1h10v10H1z" />
                    <path fill="#81bc06" d="M12 1h10v10H12z" />
                    <path fill="#05a6f0" d="M1 12h10v10H1z" />
                    <path fill="#ffba08" d="M12 12h10v10H12z" />
                  </svg>
                  Microsoft
                </button>
              </div>
            </form>
          </div>

          <div className="auth-quote">
            "Quản lý hiệu quả"
            <br />
            "Phát triển bền vững"
          </div>
        </div>
      </div>
      <div className="auth-footer">
        — MASSAGE SPA &nbsp;|&nbsp; Sức khoẻ hôm nay · Hạnh phúc ngày mai —
      </div>
    </div>
  );
}
