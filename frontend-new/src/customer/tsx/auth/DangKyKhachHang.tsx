import { useState, FormEvent } from "react";
import { useNavigate, Link } from "react-router-dom";
import { useAuth } from "../../../shared/ts/hooks/useAuth";
import api from "../../../shared/ts/api/api";
import type {
  AuthResponse,
  RegisterRequest,
} from "../../../shared/ts/types/auth";
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

type FormErrors = Partial<RegisterRequest & { confirmPassword: string }>;

export default function DangKyKhachHang() {
  const navigate = useNavigate();
  const { login } = useAuth();

  const [form, setForm] = useState<
    RegisterRequest & { confirmPassword: string }
  >({
    fullName: "",
    email: "",
    phone: "",
    password: "",
    confirmPassword: "",
  });
  const [showPwd, setShowPwd] = useState(false);
  const [showConfirmPwd, setShowConfirmPwd] = useState(false);
  const [errors, setErrors] = useState<FormErrors>({});
  const [serverError, setServerError] = useState("");
  const [loading, setLoading] = useState(false);

  const validate = (): boolean => {
    const e: FormErrors = {};
    if (!form.fullName.trim()) e.fullName = "Họ tên là bắt buộc";
    if (!form.email.trim()) e.email = "Email là bắt buộc";
    else if (!/\S+@\S+\.\S+/.test(form.email)) e.email = "Email không hợp lệ";
    if (!form.phone.trim()) e.phone = "Số điện thoại là bắt buộc";
    else if (!/^[0-9]{9,11}$/.test(form.phone.replace(/\s/g, "")))
      e.phone = "Số điện thoại không hợp lệ";
    if (!form.password) e.password = "Mật khẩu là bắt buộc";
    else if (form.password.length < 6)
      e.password = "Mật khẩu tối thiểu 6 ký tự";
    if (!form.confirmPassword) e.confirmPassword = "Vui lòng xác nhận mật khẩu";
    else if (form.password !== form.confirmPassword)
      e.confirmPassword = "Mật khẩu không khớp";
    setErrors(e);
    return Object.keys(e).length === 0;
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setServerError("");
    if (!validate()) return;
    setLoading(true);
    try {
      const payload: RegisterRequest = {
        fullName: form.fullName,
        email: form.email,
        phone: form.phone,
        password: form.password,
      };
      const { data } = await api.post<AuthResponse>("/auth/register", payload);
      login(data.token, {
        id: data.userId,
        fullName: data.fullName,
        email: data.email,
        role: data.role,
      });
      navigate("/customer", { replace: true });
    } catch (err: unknown) {
      const e = err as { response?: { data?: { message?: string } } };
      setServerError(
        e.response?.data?.message || "Đăng ký thất bại. Vui lòng thử lại.",
      );
    } finally {
      setLoading(false);
    }
  };

  const setField =
    (field: keyof typeof form) => (e: React.ChangeEvent<HTMLInputElement>) =>
      setForm((f) => ({ ...f, [field]: e.target.value }));

  return (
    <div className="auth-page">
      <div
        className="auth-bg"
        style={{
          background:
            "linear-gradient(135deg, rgba(40,15,5,0.85) 0%, rgba(80,30,10,0.75) 50%, rgba(120,60,20,0.6) 100%), url(https://images.unsplash.com/photo-1540555700478-4be289fbecef?w=1400&q=80) center/cover no-repeat",
        }}
      />
      <div
        className="auth-container"
        style={{ paddingTop: 60, paddingBottom: 60 }}
      >
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
            <div className="auth-logo-mini">
              <LotusIcon />
              <span className="auth-logo-mini-name">MASSAGE SPA</span>
            </div>

            <div className="auth-header">
              <div className="auth-avatar auth-avatar--customer">
                <svg width="32" height="32" viewBox="0 0 24 24" fill="none">
                  <circle cx="12" cy="8" r="4" fill="#fff" />
                  <path
                    d="M4 20c0-4 3.58-7 8-7s8 3 8 7"
                    stroke="#fff"
                    strokeWidth="2"
                    strokeLinecap="round"
                  />
                </svg>
              </div>
              <h1 className="auth-title">Tạo tài khoản</h1>
              <p className="auth-subtitle">
                Đăng ký để trải nghiệm dịch vụ spa
              </p>
            </div>

            <form className="auth-form" onSubmit={handleSubmit} noValidate>
              {serverError && (
                <div className="auth-server-error">{serverError}</div>
              )}

              {/* Full name */}
              <div className="auth-field">
                <label className="auth-label">Họ và tên</label>
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
                    type="text"
                    className={`auth-input${errors.fullName ? " error" : ""}`}
                    placeholder="Nhập họ và tên"
                    value={form.fullName}
                    onChange={setField("fullName")}
                    autoComplete="name"
                  />
                </div>
                {errors.fullName && (
                  <span className="auth-error-text">{errors.fullName}</span>
                )}
              </div>

              {/* Email */}
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
                    className={`auth-input${errors.email ? " error" : ""}`}
                    placeholder="Nhập địa chỉ email"
                    value={form.email}
                    onChange={setField("email")}
                    autoComplete="email"
                  />
                </div>
                {errors.email && (
                  <span className="auth-error-text">{errors.email}</span>
                )}
              </div>

              {/* Phone */}
              <div className="auth-field">
                <label className="auth-label">Số điện thoại</label>
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
                        d="M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07A19.5 19.5 0 0 1 4.69 13.5 19.79 19.79 0 0 1 1.61 4.9 2 2 0 0 1 3.6 2.73h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L7.91 10a16 16 0 0 0 6 6l.94-.94a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 21.73 17.4z"
                        strokeLinecap="round"
                      />
                    </svg>
                  </span>
                  <input
                    type="tel"
                    className={`auth-input${errors.phone ? " error" : ""}`}
                    placeholder="Nhập số điện thoại"
                    value={form.phone}
                    onChange={setField("phone")}
                    autoComplete="tel"
                  />
                </div>
                {errors.phone && (
                  <span className="auth-error-text">{errors.phone}</span>
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
                    placeholder="Tối thiểu 6 ký tự"
                    value={form.password}
                    onChange={setField("password")}
                    autoComplete="new-password"
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

              {/* Confirm Password */}
              <div className="auth-field">
                <label className="auth-label">Xác nhận mật khẩu</label>
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
                    type={showConfirmPwd ? "text" : "password"}
                    className={`auth-input${errors.confirmPassword ? " error" : ""}`}
                    placeholder="Nhập lại mật khẩu"
                    value={form.confirmPassword}
                    onChange={setField("confirmPassword")}
                    autoComplete="new-password"
                  />
                  <button
                    type="button"
                    className="auth-eye"
                    onClick={() => setShowConfirmPwd((s) => !s)}
                    aria-label="Toggle confirm password"
                  >
                    {showConfirmPwd ? (
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
                {errors.confirmPassword && (
                  <span className="auth-error-text">
                    {errors.confirmPassword}
                  </span>
                )}
              </div>

              <button
                type="submit"
                className="auth-submit auth-submit--customer"
                disabled={loading}
              >
                {loading ? (
                  <span className="auth-spinner" />
                ) : (
                  <>
                    Tạo tài khoản
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

              <p className="auth-register-link">
                Đã có tài khoản?{" "}
                <Link to="/customer/dang-nhap">Đăng nhập ngay</Link>
              </p>
            </form>
          </div>

          <div className="auth-quote">
            "Đặt lịch dễ dàng"
            <br />
            "Tận hưởng trọn vẹn"
          </div>
        </div>
      </div>
      <div className="auth-footer">
        — MASSAGE SPA &nbsp;|&nbsp; Sức khoẻ hôm nay · Hạnh phúc ngày mai —
      </div>
    </div>
  );
}
