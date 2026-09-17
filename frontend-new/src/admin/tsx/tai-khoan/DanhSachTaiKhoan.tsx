import { useEffect, useState, useMemo } from "react";
import {
  Users,
  UserCheck,
  ShieldCheck,
  UserCircle,
  Search,
  RefreshCw,
  Filter,
  Plus,
  Eye,
  Pencil,
  Lock,
  Unlock,
  KeyRound,
  X,
  Mail,
  Phone,
  Calendar,
  MapPin,
  Award,
  Briefcase,
  ChevronLeft,
  ChevronRight,
  AlertTriangle,
  Check,
} from "lucide-react";
import {
  getAccounts,
  getAccountStats,
  toggleActive,
  adminResetPassword,
  createEmployee,
  createCustomer,
  updateEmployee,
  updateCustomer,
  updateAdmin,
  POSITION_OPTIONS,
} from "../../ts/accountService";
import type { AccountDto, AccountStats } from "../../ts/accountService";
import "../../css/tai-khoan.css";

// ===== Helpers =====
const avatarClass = (role: string) => {
  if (role === "ADMIN") return "tk-avatar--admin";
  if (role === "EMPLOYEE") return "tk-avatar--employee";
  return "tk-avatar--customer";
};
const initials = (name: string) =>
  name
    .split(" ")
    .slice(-2)
    .map((s) => s[0])
    .join("")
    .toUpperCase();
const fmtDate = (d?: string) =>
  d ? new Date(d).toLocaleDateString("vi-VN") : "—";
const roleLabel: Record<string, string> = {
  ADMIN: "Quản trị viên",
  EMPLOYEE: "Nhân viên",
  CUSTOMER: "Khách hàng",
};
const posLabel: Record<string, string> = {
  RECEPTIONIST: "Lễ tân",
  MASSAGE_THERAPIST: "KTV Massage",
};

const PAGE_SIZE_OPTIONS = [10, 20, 50];

type AccountFilters = {
  search: string;
  role: string;
  status: string;
  fromDate: string;
  toDate: string;
  gender: string;
};

function AccountFilterPanel({
  initialFilters,
  onApply,
  onClose,
}: {
  initialFilters: AccountFilters;
  onApply: (filters: AccountFilters) => void;
  onClose: () => void;
}) {
  const [filters, setFilters] = useState(initialFilters);
  const set = (key: keyof AccountFilters) =>
    (event: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) =>
      setFilters((current) => ({ ...current, [key]: event.target.value }));

  const reset = () =>
    setFilters({
      search: "",
      role: "",
      status: "",
      fromDate: "",
      toDate: "",
      gender: "",
    });

  return (
    <div className="tk-filter-overlay" onClick={onClose}>
      <section
        className="tk-filter-panel"
        role="dialog"
        aria-modal="true"
        aria-label="Bộ lọc tài khoản"
        onClick={(event) => event.stopPropagation()}
      >
        <div className="tk-filter-panel-header">
          <div><Filter size={20} /> <span>Bộ lọc tài khoản</span></div>
          <button onClick={onClose} aria-label="Đóng bộ lọc"><X size={18} /></button>
        </div>

        <label className="tk-filter-field">
          <span>Từ khóa</span>
          <div className="tk-filter-search">
            <Search size={15} />
            <input value={filters.search} onChange={set("search")} placeholder="Tìm theo tên, email, số điện thoại..." />
          </div>
        </label>

        <label className="tk-filter-field">
          <span>Vai trò</span>
          <select value={filters.role} onChange={set("role")}>
            <option value="">Tất cả vai trò</option>
            <option value="ADMIN">Quản trị viên</option>
            <option value="EMPLOYEE">Nhân viên</option>
            <option value="CUSTOMER">Khách hàng</option>
          </select>
        </label>

        <label className="tk-filter-field">
          <span>Trạng thái</span>
          <select value={filters.status} onChange={set("status")}>
            <option value="">Tất cả trạng thái</option>
            <option value="active">Hoạt động</option>
            <option value="locked">Đã khóa</option>
          </select>
        </label>

        <div className="tk-filter-field">
          <span>Ngày tạo</span>
          <div className="tk-filter-date-row">
            <label><Calendar size={14} /><input type="date" value={filters.fromDate} onChange={set("fromDate")} aria-label="Từ ngày" /></label>
            <label><Calendar size={14} /><input type="date" value={filters.toDate} onChange={set("toDate")} aria-label="Đến ngày" /></label>
          </div>
        </div>

        <label className="tk-filter-field">
          <span>Giới tính (tùy chọn)</span>
          <select value={filters.gender} onChange={set("gender")}>
            <option value="">Tất cả</option>
            <option value="Nam">Nam</option>
            <option value="Nữ">Nữ</option>
            <option value="Khác">Khác</option>
          </select>
        </label>

        <div className="tk-filter-panel-actions">
          <button className="tk-btn tk-btn--outline" onClick={reset}><RefreshCw size={15} /> Đặt lại</button>
          <button className="tk-btn tk-btn--primary" onClick={() => onApply(filters)}><Filter size={15} /> Áp dụng</button>
        </div>
      </section>
    </div>
  );
}

// ===== Confirm Modal =====
function ConfirmModal({
  type,
  name,
  onConfirm,
  onCancel,
}: {
  type: "lock" | "unlock" | "reset";
  name: string;
  onConfirm: () => void;
  onCancel: () => void;
}) {
  const isLock = type === "lock";
  const isReset = type === "reset";
  return (
    <div className="tk-modal-overlay" onClick={onCancel}>
      <div
        className="tk-modal"
        style={{ maxWidth: 400 }}
        onClick={(e) => e.stopPropagation()}
      >
        <div className="tk-modal-body" style={{ paddingTop: 28 }}>
          <div
            className={`tk-confirm-icon ${isLock ? "tk-confirm-icon--danger" : isReset ? "tk-confirm-icon--warning" : "tk-confirm-icon--warning"}`}
          >
            {isLock ? (
              <Lock size={22} />
            ) : isReset ? (
              <KeyRound size={22} />
            ) : (
              <Unlock size={22} />
            )}
          </div>
          <p className="tk-confirm-title">
            {isLock
              ? "Khóa tài khoản"
              : isReset
                ? "Reset mật khẩu"
                : "Mở khóa tài khoản"}
          </p>
          <p className="tk-confirm-desc">
            {isLock && (
              <>
                Tài khoản <strong>{name}</strong> sẽ bị khóa và không thể đăng
                nhập.
              </>
            )}
            {type === "unlock" && (
              <>
                Tài khoản <strong>{name}</strong> sẽ được mở khóa.
              </>
            )}
            {isReset && (
              <>
                Mật khẩu của <strong>{name}</strong> sẽ được đặt lại thành{" "}
                <strong>Spa@123456</strong>.
              </>
            )}
          </p>
        </div>
        <div className="tk-modal-footer">
          <button className="tk-btn tk-btn--outline" onClick={onCancel}>
            Hủy
          </button>
          <button
            className="tk-btn tk-btn--primary"
            style={{
              background: isLock ? "#dc2626" : isReset ? "#d97706" : "#16a34a",
            }}
            onClick={onConfirm}
          >
            {isLock ? "Khóa" : isReset ? "Reset" : "Mở khóa"}
          </button>
        </div>
      </div>
    </div>
  );
}

// ===== Create/Edit Modal =====
function AccountModal({
  mode,
  account,
  onClose,
  onSaved,
}: {
  mode: "create-employee" | "create-customer" | "edit";
  account?: AccountDto;
  onClose: () => void;
  onSaved: () => void;
}) {
  const isEdit = mode === "edit";
  const isEmp =
    mode === "create-employee" || (isEdit && account?.role === "EMPLOYEE");

  const [form, setForm] = useState({
    fullName: account?.fullName ?? "",
    email: account?.email ?? "",
    phone: account?.phone ?? "",
    password: "",
    position: account?.position ?? "",
    startDate:
      account?.startDate?.slice(0, 10) ?? new Date().toISOString().slice(0, 10),
    address: account?.address ?? "",
    dateOfBirth: account?.dateOfBirth?.slice(0, 10) ?? "",
    gender: account?.gender ?? "",
  });
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [loading, setLoading] = useState(false);
  const [serverError, setServerError] = useState("");

  const set =
    (k: string) =>
    (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) =>
      setForm((f) => ({ ...f, [k]: e.target.value }));

  const validate = () => {
    const e: Record<string, string> = {};
    if (!form.fullName.trim()) e.fullName = "Họ tên là bắt buộc";
    if (!isEdit && !form.email.trim()) e.email = "Email là bắt buộc";
    if (!isEdit && !form.phone.trim()) e.phone = "SĐT là bắt buộc";
    if (!isEdit && form.password.length < 6) e.password = "Tối thiểu 6 ký tự";
    setErrors(e);
    return Object.keys(e).length === 0;
  };

  const handleSave = async () => {
    if (!validate()) return;
    setLoading(true);
    setServerError("");
    try {
      if (isEdit && account) {
        if (account.role === "ADMIN") {
          await updateAdmin(account.userId, {
            fullName: form.fullName,
            phone: form.phone,
          });
        } else if (account.role === "EMPLOYEE" && account.employeeId) {
          await updateEmployee(account.employeeId, {
            fullName: form.fullName,
            phone: form.phone,
            position: form.position || undefined,
          });
        } else if (account.role === "CUSTOMER" && account.customerId) {
          await updateCustomer(account.customerId, {
            fullName: form.fullName,
            phone: form.phone,
            address: form.address || undefined,
            dateOfBirth: form.dateOfBirth || undefined,
            gender: form.gender || undefined,
          });
        }
      } else if (mode === "create-employee") {
        await createEmployee({
          fullName: form.fullName,
          email: form.email,
          phone: form.phone,
          password: form.password,
          position: form.position || undefined,
          startDate: form.startDate,
        });
      } else {
        await createCustomer({
          fullName: form.fullName,
          email: form.email,
          phone: form.phone,
          password: form.password,
          address: form.address,
          dateOfBirth: form.dateOfBirth || undefined,
          gender: form.gender,
        });
      }
      onSaved();
    } catch (err: unknown) {
      const e = err as { response?: { data?: { message?: string } } };
      setServerError(e.response?.data?.message ?? "Có lỗi xảy ra");
    } finally {
      setLoading(false);
    }
  };

  const title = isEdit
    ? "Sửa tài khoản"
    : isEmp
      ? "Thêm nhân viên"
      : "Thêm khách hàng";

  return (
    <div className="tk-modal-overlay" onClick={onClose}>
      <div className="tk-modal" onClick={(e) => e.stopPropagation()}>
        <div className="tk-modal-header">
          <span className="tk-modal-title">{title}</span>
          <button className="tk-modal-close" onClick={onClose}>
            <X size={15} />
          </button>
        </div>
        <div className="tk-modal-body">
          {serverError && (
            <div
              style={{
                background: "#fef2f2",
                border: "1px solid #fecaca",
                color: "#dc2626",
                borderRadius: 8,
                padding: "10px 14px",
                fontSize: 13,
                marginBottom: 14,
              }}
            >
              {serverError}
            </div>
          )}
          <div className="tk-form-grid">
            <div className="tk-field tk-form-full">
              <label className="tk-label">
                Họ và tên <span>*</span>
              </label>
              <input
                className={`tk-input${errors.fullName ? " error" : ""}`}
                value={form.fullName}
                onChange={set("fullName")}
                placeholder="Nhập họ và tên"
              />
              {errors.fullName && (
                <span className="tk-error">{errors.fullName}</span>
              )}
            </div>

            {!isEdit && (
              <>
                <div className="tk-field">
                  <label className="tk-label">
                    Email <span>*</span>
                  </label>
                  <input
                    className={`tk-input${errors.email ? " error" : ""}`}
                    type="email"
                    value={form.email}
                    onChange={set("email")}
                    placeholder="email@example.com"
                  />
                  {errors.email && (
                    <span className="tk-error">{errors.email}</span>
                  )}
                </div>
                <div className="tk-field">
                  <label className="tk-label">
                    Số điện thoại <span>*</span>
                  </label>
                  <input
                    className={`tk-input${errors.phone ? " error" : ""}`}
                    value={form.phone}
                    onChange={set("phone")}
                    placeholder="0912345678"
                  />
                  {errors.phone && (
                    <span className="tk-error">{errors.phone}</span>
                  )}
                </div>
                <div className="tk-field tk-form-full">
                  <label className="tk-label">
                    Mật khẩu <span>*</span>
                  </label>
                  <input
                    className={`tk-input${errors.password ? " error" : ""}`}
                    type="password"
                    value={form.password}
                    onChange={set("password")}
                    placeholder="Tối thiểu 6 ký tự"
                  />
                  {errors.password && (
                    <span className="tk-error">{errors.password}</span>
                  )}
                </div>
              </>
            )}

            {isEdit && (
              <div className="tk-field">
                <label className="tk-label">Số điện thoại</label>
                <input
                  className="tk-input"
                  value={form.phone}
                  onChange={set("phone")}
                  placeholder="0912345678"
                />
              </div>
            )}

            {isEmp && (
              <>
                <div className="tk-field">
                  <label className="tk-label">Chức vụ</label>
                  <select
                    className="tk-input"
                    value={form.position}
                    onChange={set("position")}
                    style={{ appearance: "auto" }}
                  >
                    <option value="">-- Chọn chức vụ --</option>
                    {POSITION_OPTIONS.map((p) => (
                      <option key={p.value} value={p.value}>
                        {p.label}
                      </option>
                    ))}
                  </select>
                </div>
                {!isEdit && (
                  <div className="tk-field">
                    <label className="tk-label">Ngày vào làm</label>
                    <input
                      className="tk-input"
                      type="date"
                      value={form.startDate}
                      onChange={set("startDate")}
                    />
                  </div>
                )}
              </>
            )}

            {(!isEdit || account?.role === "CUSTOMER") && !isEmp && (
              <>
                <div className="tk-field">
                  <label className="tk-label">Giới tính</label>
                  <select
                    className="tk-input"
                    value={form.gender}
                    onChange={set("gender")}
                    style={{ appearance: "auto" }}
                  >
                    <option value="">-- Chọn --</option>
                    <option value="Nam">Nam</option>
                    <option value="Nữ">Nữ</option>
                    <option value="Khác">Khác</option>
                  </select>
                </div>
                <div className="tk-field tk-form-full">
                  <label className="tk-label">Địa chỉ</label>
                  <input
                    className="tk-input"
                    value={form.address}
                    onChange={set("address")}
                    placeholder="Địa chỉ"
                  />
                </div>
                <div className="tk-field">
                  <label className="tk-label">Ngày sinh</label>
                  <input
                    className="tk-input"
                    type="date"
                    value={form.dateOfBirth}
                    onChange={set("dateOfBirth")}
                  />
                </div>
              </>
            )}
          </div>
        </div>
        <div className="tk-modal-footer">
          <button className="tk-btn tk-btn--outline" onClick={onClose}>
            Hủy
          </button>
          <button
            className="tk-btn tk-btn--primary"
            onClick={handleSave}
            disabled={loading}
          >
            {loading ? "..." : isEdit ? "Lưu thay đổi" : "Thêm tài khoản"}
          </button>
        </div>
      </div>
    </div>
  );
}

// ===== Detail Panel =====
function DetailPanel({
  account,
  onClose,
  onAction,
}: {
  account: AccountDto;
  onClose: () => void;
  onAction: (action: "lock" | "unlock" | "reset" | "edit") => void;
}) {
  const [tab, setTab] = useState<"info" | "history">("info");
  const isEmp = account.role === "EMPLOYEE";
  const isCust = account.role === "CUSTOMER";

  return (
    <div className="tk-detail">
      <div className="tk-detail-header">
        <span className="tk-detail-title">Thông tin tài khoản</span>
        <button className="tk-detail-close" onClick={onClose}>
          <X size={13} />
        </button>
      </div>

      <div className="tk-detail-user">
        <div className={`tk-detail-avatar ${avatarClass(account.role)}`}>
          {initials(account.fullName)}
        </div>
        <div style={{ flex: 1 }}>
          <div className="tk-detail-name">{account.fullName}</div>
          <div className="tk-detail-meta">
            <span className={`tk-role tk-role--${account.role}`}>
              {roleLabel[account.role]}
            </span>
            <span
              className={`tk-status ${account.isActive ? "tk-status--active" : "tk-status--locked"}`}
            >
              <span className="tk-status-dot" />
              {account.isActive ? "Hoạt động" : "Đã khóa"}
            </span>
          </div>
          <div style={{ fontSize: 11, color: "#94a3b8", marginTop: 4 }}>
            Tham gia: {fmtDate(account.createdAt)}
          </div>
        </div>
      </div>

      <div className="tk-detail-tabs">
        <button
          className={`tk-detail-tab${tab === "info" ? " active" : ""}`}
          onClick={() => setTab("info")}
        >
          Thông tin chung
        </button>
        <button
          className={`tk-detail-tab${tab === "history" ? " active" : ""}`}
          onClick={() => setTab("history")}
        >
          Lịch sử hoạt động
        </button>
      </div>

      {tab === "info" && (
        <div className="tk-detail-body">
          <div className="tk-info-row">
            <UserCircle size={14} className="tk-info-icon" />
            <span className="tk-info-label">Họ và tên</span>
            <span className="tk-info-val">{account.fullName}</span>
          </div>
          <div className="tk-info-row">
            <Mail size={14} className="tk-info-icon" />
            <span className="tk-info-label">Email</span>
            <span className="tk-info-val">{account.email}</span>
          </div>
          <div className="tk-info-row">
            <Phone size={14} className="tk-info-icon" />
            <span className="tk-info-label">Số điện thoại</span>
            <span className="tk-info-val">{account.phone}</span>
          </div>
          <div className="tk-info-row">
            <ShieldCheck size={14} className="tk-info-icon" />
            <span className="tk-info-label">Vai trò</span>
            <span className="tk-info-val">{roleLabel[account.role]}</span>
          </div>
          <div className="tk-info-row">
            <Check size={14} className="tk-info-icon" />
            <span className="tk-info-label">Trạng thái</span>
            <span className="tk-info-val">
              {account.isActive ? "Hoạt động" : "Đã khóa"}
            </span>
          </div>
          <div className="tk-info-row">
            <Calendar size={14} className="tk-info-icon" />
            <span className="tk-info-label">Ngày tạo</span>
            <span className="tk-info-val">{fmtDate(account.createdAt)}</span>
          </div>
          {isEmp && account.position && (
            <div className="tk-info-row">
              <Briefcase size={14} className="tk-info-icon" />
              <span className="tk-info-label">Chức vụ</span>
              <span className="tk-info-val">
                {posLabel[account.position] ?? account.position}
              </span>
            </div>
          )}
          {isEmp && account.startDate && (
            <div className="tk-info-row">
              <Calendar size={14} className="tk-info-icon" />
              <span className="tk-info-label">Ngày vào làm</span>
              <span className="tk-info-val">{fmtDate(account.startDate)}</span>
            </div>
          )}
          {isCust && (
            <>
              <div className="tk-info-row">
                <Award size={14} className="tk-info-icon" />
                <span className="tk-info-label">Điểm tích lũy</span>
                <span className="tk-info-val">
                  {account.loyaltyPoints ?? 0} điểm
                </span>
              </div>
              <div className="tk-info-row">
                <UserCircle size={14} className="tk-info-icon" />
                <span className="tk-info-label">Giới tính</span>
                <span className="tk-info-val">{account.gender ?? "—"}</span>
              </div>
              <div className="tk-info-row">
                <Calendar size={14} className="tk-info-icon" />
                <span className="tk-info-label">Ngày sinh</span>
                <span className="tk-info-val">{fmtDate(account.dateOfBirth)}</span>
              </div>
              <div className="tk-info-row">
                <MapPin size={14} className="tk-info-icon" />
                <span className="tk-info-label">Địa chỉ</span>
                <span className="tk-info-val">{account.address ?? "—"}</span>
              </div>
            </>
          )}
        </div>
      )}

      {tab === "history" && (
        <div className="tk-detail-body">
          <p
            style={{
              color: "#94a3b8",
              fontSize: 13,
              textAlign: "center",
              padding: "20px 0",
            }}
          >
            Chức năng lịch sử hoạt động đang phát triển
          </p>
        </div>
      )}

      <div className="tk-detail-actions">
        <button
          className="tk-detail-btn tk-detail-btn--blue"
          onClick={() => onAction("edit")}
        >
          <Pencil size={13} /> Chỉnh sửa
        </button>
        <button
          className="tk-detail-btn tk-detail-btn--orange"
          onClick={() => onAction("reset")}
        >
          <KeyRound size={13} /> Đổi mật khẩu
        </button>
        {account.isActive ? (
          <button
            className="tk-detail-btn tk-detail-btn--red"
            onClick={() => onAction("lock")}
            style={{ gridColumn: "1 / -1" }}
          >
            <Lock size={13} /> Khóa tài khoản
          </button>
        ) : (
          <button
            className="tk-detail-btn tk-detail-btn--green"
            onClick={() => onAction("unlock")}
            style={{ gridColumn: "1 / -1" }}
          >
            <Unlock size={13} /> Mở khóa tài khoản
          </button>
        )}
      </div>
    </div>
  );
}

// ===== MAIN PAGE =====
export default function DanhSachTaiKhoan() {
  const [accounts, setAccounts] = useState<AccountDto[]>([]);
  const [stats, setStats] = useState<AccountStats | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // Filters
  const [search, setSearch] = useState("");
  const [roleFilter, setRoleFilter] = useState("");
  const [statusFilter, setStatusFilter] = useState("");
  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");
  const [genderFilter, setGenderFilter] = useState("");

  // Pagination
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  // Detail panel
  const [selectedAccount, setSelectedAccount] = useState<AccountDto | null>(
    null,
  );

  // Modals
  const [confirmAction, setConfirmAction] = useState<{
    type: "lock" | "unlock" | "reset";
    account: AccountDto;
  } | null>(null);
  const [modal, setModal] = useState<{
    mode: "create-employee" | "create-customer" | "edit";
    account?: AccountDto;
  } | null>(null);
  const [showCreateMenu, setShowCreateMenu] = useState(false);
  const [showFilterPanel, setShowFilterPanel] = useState(false);
  const [toastMsg, setToastMsg] = useState("");

  const showToast = (msg: string) => {
    setToastMsg(msg);
    setTimeout(() => setToastMsg(""), 3000);
  };

  const loadData = async () => {
    try {
      setLoading(true);
      const [accs, st] = await Promise.all([getAccounts(), getAccountStats()]);
      setAccounts(accs);
      setStats(st);
    } catch {
      setError("Không thể tải danh sách tài khoản");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  // Client-side filter
  const filtered = useMemo(() => {
    let list = accounts;
    if (roleFilter) list = list.filter((a) => a.role === roleFilter);
    if (statusFilter === "active") list = list.filter((a) => a.isActive);
    if (statusFilter === "locked") list = list.filter((a) => !a.isActive);
    if (genderFilter) list = list.filter((a) => a.gender === genderFilter);
    if (fromDate) {
      const from = new Date(`${fromDate}T00:00:00`);
      list = list.filter((a) => new Date(a.createdAt) >= from);
    }
    if (toDate) {
      const to = new Date(`${toDate}T23:59:59.999`);
      list = list.filter((a) => new Date(a.createdAt) <= to);
    }
    if (search.trim()) {
      const s = search.toLowerCase();
      list = list.filter(
        (a) =>
          a.fullName.toLowerCase().includes(s) ||
          a.email.toLowerCase().includes(s) ||
          a.phone.includes(s),
      );
    }
    return list;
  }, [accounts, roleFilter, statusFilter, search, fromDate, toDate, genderFilter]);

  const applyFilters = (filters: AccountFilters) => {
    setSearch(filters.search);
    setRoleFilter(filters.role);
    setStatusFilter(filters.status);
    setFromDate(filters.fromDate);
    setToDate(filters.toDate);
    setGenderFilter(filters.gender);
    setPage(1);
    setShowFilterPanel(false);
  };

  const totalPages = Math.ceil(filtered.length / pageSize);
  const paginated = filtered.slice((page - 1) * pageSize, page * pageSize);

  const handleConfirm = async () => {
    if (!confirmAction) return;
    const { type, account } = confirmAction;
    try {
      if (type === "lock") await toggleActive(account.userId, false);
      else if (type === "unlock") await toggleActive(account.userId, true);
      else if (type === "reset")
        await adminResetPassword(account.userId, "Spa@123456");
      setConfirmAction(null);
      showToast(
        type === "lock"
          ? "Đã khóa tài khoản"
          : type === "unlock"
            ? "Đã mở khóa tài khoản"
            : "Đã reset mật khẩu thành Spa@123456",
      );
      await loadData();
      if (selectedAccount?.userId === account.userId) setSelectedAccount(null);
    } catch (err: unknown) {
      const e = err as { response?: { data?: { message?: string } } };
      showToast(e.response?.data?.message ?? "Có lỗi xảy ra");
      setConfirmAction(null);
    }
  };

  const handleDetailAction = (action: "lock" | "unlock" | "reset" | "edit") => {
    if (!selectedAccount) return;
    if (action === "edit") {
      setModal({ mode: "edit", account: selectedAccount });
    } else {
      setConfirmAction({ type: action, account: selectedAccount });
    }
  };

  const handleSaved = async () => {
    setModal(null);
    showToast("Lưu thành công");
    await loadData();
  };

  return (
    <div className="tk-page">
      {/* Toast */}
      {toastMsg && (
        <div
          style={{
            position: "fixed",
            top: 20,
            right: 20,
            zIndex: 2000,
            background: "#0f172a",
            color: "#fff",
            padding: "12px 18px",
            borderRadius: 10,
            fontSize: 13.5,
            fontWeight: 600,
            boxShadow: "0 8px 24px rgba(0,0,0,0.2)",
            display: "flex",
            alignItems: "center",
            gap: 8,
          }}
        >
          <Check size={15} color="#22c55e" /> {toastMsg}
        </div>
      )}

      {/* Header */}
      <div className="tk-header">
        <div>
          <h1 className="tk-title">
            <Users size={22} /> Quản lý tài khoản
          </h1>
          <p className="tk-sub">
            Quản lý tất cả tài khoản trong hệ thống. Tạo, chỉnh sửa, phân quyền
            và khóa/mở khóa tài khoản.
          </p>
        </div>
        <div style={{ position: "relative" }}>
          <button
            className="tk-btn tk-btn--primary"
            onClick={() => setShowCreateMenu((visible) => !visible)}
          >
            <Plus size={16} /> Thêm tài khoản
          </button>
          {showCreateMenu && (
            <div className="tk-create-menu">
              <button
                onClick={() => {
                  setShowCreateMenu(false);
                  setModal({ mode: "create-employee" });
                }}
              >
                <UserCheck size={15} /> Thêm nhân viên
              </button>
              <button
                onClick={() => {
                  setShowCreateMenu(false);
                  setModal({ mode: "create-customer" });
                }}
              >
                <UserCircle size={15} /> Thêm khách hàng
              </button>
            </div>
          )}
        </div>
      </div>

      {/* Stats */}
      <div className="tk-stats">
        <div className="tk-stat">
          <div className="tk-stat-icon tk-stat-icon--blue">
            <Users size={20} />
          </div>
          <div>
            <div className="tk-stat-val">{stats?.total ?? 0}</div>
            <div className="tk-stat-label">Tổng tài khoản</div>
          </div>
        </div>
        <div className="tk-stat">
          <div className="tk-stat-icon tk-stat-icon--purple">
            <ShieldCheck size={20} />
          </div>
          <div>
            <div className="tk-stat-val">{stats?.admins ?? 0}</div>
            <div className="tk-stat-label">Quản trị viên</div>
          </div>
        </div>
        <div className="tk-stat">
          <div className="tk-stat-icon tk-stat-icon--green">
            <UserCheck size={20} />
          </div>
          <div>
            <div className="tk-stat-val">{stats?.employees ?? 0}</div>
            <div className="tk-stat-label">Nhân viên</div>
          </div>
        </div>
        <div className="tk-stat">
          <div className="tk-stat-icon tk-stat-icon--orange">
            <UserCircle size={20} />
          </div>
          <div>
            <div className="tk-stat-val">{stats?.customers ?? 0}</div>
            <div className="tk-stat-label">Khách hàng</div>
          </div>
        </div>
      </div>

      {/* Filters */}
      <div className="tk-filters">
        <div className="tk-search-wrap">
          <Search size={14} className="tk-search-icon" />
          <input
            className="tk-search"
            placeholder="Tìm kiếm theo tên, email, số điện thoại..."
            value={search}
            onChange={(e) => {
              setSearch(e.target.value);
              setPage(1);
            }}
          />
        </div>
        <select
          className="tk-select"
          value={roleFilter}
          onChange={(e) => {
            setRoleFilter(e.target.value);
            setPage(1);
          }}
        >
          <option value="">Tất cả vai trò</option>
          <option value="ADMIN">Quản trị viên</option>
          <option value="EMPLOYEE">Nhân viên</option>
          <option value="CUSTOMER">Khách hàng</option>
        </select>
        <select
          className="tk-select"
          value={statusFilter}
          onChange={(e) => {
            setStatusFilter(e.target.value);
            setPage(1);
          }}
        >
          <option value="">Tất cả trạng thái</option>
          <option value="active">Hoạt động</option>
          <option value="locked">Đã khóa</option>
        </select>
        <div className="tk-filter-actions">
          <button className="tk-btn tk-btn--outline" onClick={loadData}>
            <RefreshCw size={13} /> Làm mới
          </button>
          <button className="tk-btn tk-btn--outline" onClick={() => setShowFilterPanel(true)}>
            <Filter size={13} /> Lọc
          </button>
        </div>
      </div>

      {/* Content */}
      <div className={`tk-content${!selectedAccount ? " no-detail" : ""}`}>
        {/* Table */}
        <div className="tk-table-card">
          <div className="tk-table-header">
            <span className="tk-table-title">Danh sách tài khoản</span>
            <span className="tk-table-count">
              ({filtered.length} tài khoản)
            </span>
          </div>

          {loading ? (
            <div className="tk-loading">
              <span className="tk-spinner" /> Đang tải...
            </div>
          ) : error ? (
            <div className="tk-loading" style={{ color: "#ef4444" }}>
              <AlertTriangle size={16} /> {error}
            </div>
          ) : (
            <>
              <div className="tk-table-wrap">
                <table className="tk-table">
                  <thead>
                    <tr>
                      <th>#</th>
                      <th>Họ và tên</th>
                      <th>Email</th>
                      <th>Số điện thoại</th>
                      <th>Vai trò</th>
                      <th>Chức vụ</th>
                      <th>Trạng thái</th>
                      <th>Ngày tạo</th>
                      <th>Thao tác</th>
                    </tr>
                  </thead>
                  <tbody>
                    {paginated.length === 0 ? (
                      <tr>
                        <td colSpan={9} className="tk-empty">
                          Không tìm thấy tài khoản nào
                        </td>
                      </tr>
                    ) : (
                      paginated.map((acc, i) => (
                        <tr
                          key={acc.userId}
                          className={
                            selectedAccount?.userId === acc.userId
                              ? "selected"
                              : ""
                          }
                          onClick={() =>
                            setSelectedAccount(
                              acc.userId === selectedAccount?.userId
                                ? null
                                : acc,
                            )
                          }
                        >
                          <td style={{ color: "#94a3b8" }}>
                            {(page - 1) * pageSize + i + 1}
                          </td>
                          <td>
                            <div className="tk-avatar-cell">
                              <div
                                className={`tk-avatar ${avatarClass(acc.role)}`}
                              >
                                {initials(acc.fullName)}
                              </div>
                              <div>
                                <div className="tk-name">{acc.fullName}</div>
                                <div className="tk-username">
                                  {acc.email.split("@")[0]}
                                </div>
                              </div>
                            </div>
                          </td>
                          <td>{acc.email}</td>
                          <td>{acc.phone}</td>
                          <td>
                            <span className={`tk-role tk-role--${acc.role}`}>
                              {roleLabel[acc.role]}
                            </span>
                          </td>
                          <td style={{ color: "#64748b", fontSize: 12 }}>
                            {acc.position
                              ? (posLabel[acc.position] ?? acc.position)
                              : "—"}
                          </td>
                          <td>
                            <span
                              className={`tk-status ${acc.isActive ? "tk-status--active" : "tk-status--locked"}`}
                            >
                              <span className="tk-status-dot" />
                              {acc.isActive ? "Hoạt động" : "Đã khóa"}
                            </span>
                          </td>
                          <td style={{ color: "#64748b" }}>
                            {fmtDate(acc.createdAt)}
                          </td>
                          <td onClick={(e) => e.stopPropagation()}>
                            <div className="tk-actions">
                              <button
                                className="tk-action-btn"
                                title="Xem"
                                onClick={() => setSelectedAccount(acc)}
                              >
                                <Eye size={13} />
                              </button>
                              <button
                                className="tk-action-btn"
                                title="Sửa"
                                onClick={() =>
                                  setModal({ mode: "edit", account: acc })
                                }
                              >
                                <Pencil size={13} />
                              </button>
                              {acc.isActive ? (
                                <button
                                  className="tk-action-btn tk-action-btn--danger"
                                  title="Khóa"
                                  onClick={() =>
                                    setConfirmAction({
                                      type: "lock",
                                      account: acc,
                                    })
                                  }
                                >
                                  <Lock size={13} />
                                </button>
                              ) : (
                                <button
                                  className="tk-action-btn"
                                  title="Mở khóa"
                                  onClick={() =>
                                    setConfirmAction({
                                      type: "unlock",
                                      account: acc,
                                    })
                                  }
                                >
                                  <Unlock size={13} />
                                </button>
                              )}
                            </div>
                          </td>
                        </tr>
                      ))
                    )}
                  </tbody>
                </table>
              </div>

              {/* Pagination */}
              <div className="tk-pagination">
                <span className="tk-pag-info">
                  Hiển thị{" "}
                  {Math.min((page - 1) * pageSize + 1, filtered.length)}–
                  {Math.min(page * pageSize, filtered.length)} trong tổng số{" "}
                  {filtered.length} tài khoản
                </span>
                <div className="tk-pag-pages">
                  <button
                    className="tk-pag-btn"
                    disabled={page === 1}
                    onClick={() => setPage((p) => p - 1)}
                  >
                    <ChevronLeft size={13} />
                  </button>
                  {Array.from({ length: Math.min(totalPages, 5) }, (_, i) => {
                    const p =
                      totalPages <= 5
                        ? i + 1
                        : page <= 3
                          ? i + 1
                          : page >= totalPages - 2
                            ? totalPages - 4 + i
                            : page - 2 + i;
                    return (
                      <button
                        key={p}
                        className={`tk-pag-btn${page === p ? " active" : ""}`}
                        onClick={() => setPage(p)}
                      >
                        {p}
                      </button>
                    );
                  })}
                  <button
                    className="tk-pag-btn"
                    disabled={page === totalPages || totalPages === 0}
                    onClick={() => setPage((p) => p + 1)}
                  >
                    <ChevronRight size={13} />
                  </button>
                  <select
                    className="tk-pag-select"
                    value={pageSize}
                    onChange={(e) => {
                      setPageSize(Number(e.target.value));
                      setPage(1);
                    }}
                  >
                    {PAGE_SIZE_OPTIONS.map((n) => (
                      <option key={n} value={n}>
                        Hiển thị {n}
                      </option>
                    ))}
                  </select>
                </div>
              </div>
            </>
          )}
        </div>

        {/* Detail Panel */}
        {selectedAccount && (
          <DetailPanel
            account={selectedAccount}
            onClose={() => setSelectedAccount(null)}
            onAction={handleDetailAction}
          />
        )}
      </div>

      {/* Modals */}
      {confirmAction && (
        <ConfirmModal
          type={confirmAction.type}
          name={confirmAction.account.fullName}
          onConfirm={handleConfirm}
          onCancel={() => setConfirmAction(null)}
        />
      )}
      {modal && (
        <AccountModal
          mode={modal.mode}
          account={modal.account}
          onClose={() => setModal(null)}
          onSaved={handleSaved}
        />
      )}
      {showFilterPanel && (
        <AccountFilterPanel
          initialFilters={{ search, role: roleFilter, status: statusFilter, fromDate, toDate, gender: genderFilter }}
          onApply={applyFilters}
          onClose={() => setShowFilterPanel(false)}
        />
      )}
    </div>
  );
}
