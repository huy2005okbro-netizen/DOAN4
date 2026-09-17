import { Outlet, useNavigate, useLocation } from "react-router-dom";
import { useState } from "react";
import {
  LayoutDashboard,
  Users,
  UserCheck,
  Scissors,
  DoorOpen,
  CalendarClock,
  Tag,
  ShoppingBag,
  Warehouse,
  ShoppingCart,
  Truck,
  CreditCard,
  FileText,
  Gift,
  Star,
  BarChart3,
  Settings,
  Bell,
  Search,
  LogOut,
  ChevronDown,
} from "lucide-react";
import { useAuth } from "../../../shared/ts/hooks/useAuth";
import "../../css/layout.css";

const LotusLogo = () => (
  <svg className="sidebar-logo-icon" viewBox="0 0 40 40" fill="none">
    <g transform="translate(20,22)">
      <ellipse cx="0" cy="-8" rx="4" ry="9" fill="#d4a84b" />
      <ellipse
        cx="0"
        cy="-7"
        rx="3.5"
        ry="8"
        fill="#f0c060"
        opacity="0.9"
        transform="rotate(30)"
      />
      <ellipse
        cx="0"
        cy="-7"
        rx="3.5"
        ry="8"
        fill="#f0c060"
        opacity="0.9"
        transform="rotate(-30)"
      />
      <ellipse
        cx="0"
        cy="-6"
        rx="3"
        ry="7"
        fill="#d4a84b"
        opacity="0.75"
        transform="rotate(60)"
      />
      <ellipse
        cx="0"
        cy="-6"
        rx="3"
        ry="7"
        fill="#d4a84b"
        opacity="0.75"
        transform="rotate(-60)"
      />
      <rect
        x="-1"
        y="1"
        width="2"
        height="5"
        rx="1"
        fill="#d4a84b"
        opacity="0.8"
      />
      <ellipse
        cx="-4"
        cy="4"
        rx="4"
        ry="1.8"
        fill="#d4a84b"
        opacity="0.7"
        transform="rotate(-20,-4,4)"
      />
      <ellipse
        cx="4"
        cy="4"
        rx="4"
        ry="1.8"
        fill="#d4a84b"
        opacity="0.7"
        transform="rotate(20,4,4)"
      />
    </g>
  </svg>
);

interface NavItem {
  icon: React.ReactNode;
  label: string;
  path: string;
  badge?: number;
  section?: string;
  children?: { label: string; path: string }[];
}

const NAV_ITEMS: NavItem[] = [
  {
    section: "TỔNG QUAN",
    icon: <LayoutDashboard size={16} />,
    label: "Tổng quan",
    path: "/admin",
  },
  {
    section: "QUẢN LÝ",
    icon: <Users size={16} />,
    label: "Quản lý tài khoản",
    path: "/admin/tai-khoan",
  },
  {
    icon: <Users size={16} />,
      label: "Quản lý khách hàng",
    path: "/admin/khach-hang",
    children: [
      { label: "Danh sách khách hàng", path: "/admin/khach-hang" },
      { label: "Lịch sử đặt lịch", path: "/admin/khach-hang?view=appointments" },
      { label: "Lịch sử mua hàng", path: "/admin/khach-hang?view=orders" },
    ],
  },
  {
    icon: <UserCheck size={16} />,
    label: "Nhân viên",
    path: "/admin/nhan-vien",
  },
  {
    section: "DỊCH VỤ",
    icon: <Scissors size={16} />,
    label: "Dịch vụ Massage",
    path: "/admin/dich-vu",
  },
  {
    icon: <DoorOpen size={16} />,
    label: "Phòng Massage",
    path: "/admin/phong",
  },
  {
    icon: <CalendarClock size={16} />,
    label: "Lịch hẹn",
    path: "/admin/lich-hen",
  },
  {
    section: "SẢN PHẨM",
    icon: <Tag size={16} />,
    label: "Danh mục sản phẩm",
    path: "/admin/danh-muc-san-pham",
  },
  {
    icon: <ShoppingBag size={16} />,
    label: "Sản phẩm",
    path: "/admin/san-pham",
  },
  { icon: <Warehouse size={16} />, label: "Kho hàng", path: "/admin/kho" },
  {
    section: "BÁN HÀNG",
    icon: <ShoppingCart size={16} />,
    label: "Đơn hàng",
    path: "/admin/don-hang",
  },
  { icon: <Truck size={16} />, label: "Vận chuyển", path: "/admin/van-chuyen" },
  {
    icon: <CreditCard size={16} />,
    label: "Thanh toán",
    path: "/admin/thanh-toan",
  },
  { icon: <FileText size={16} />, label: "Hóa đơn", path: "/admin/hoa-don" },
  {
    section: "TIỆN ÍCH",
    icon: <Gift size={16} />,
    label: "Khuyến mãi",
    path: "/admin/khuyen-mai",
  },
  { icon: <Star size={16} />, label: "Đánh giá", path: "/admin/danh-gia" },
  {
    icon: <BarChart3 size={16} />,
    label: "Báo cáo thống kê",
    path: "/admin/bao-cao",
  },
  {
    section: "HỆ THỐNG",
    icon: <Settings size={16} />,
    label: "Cài đặt",
    path: "/admin/cai-dat",
  },
];

export default function AdminLayout() {
  const navigate = useNavigate();
  const location = useLocation();
  const { user, logout } = useAuth();
  const [showUserMenu, setShowUserMenu] = useState(false);
  const [expandedMenu, setExpandedMenu] = useState<string | null>(
    location.pathname.startsWith("/admin/khach-hang") ? "/admin/khach-hang" : null,
  );

  const isActive = (path: string) => {
    if (path === "/admin") return location.pathname === "/admin";
    return location.pathname.startsWith(path);
  };

  const handleLogout = () => {
    logout();
    navigate("/");
  };

  return (
    <div className="admin-wrapper">
      {/* SIDEBAR */}
      <aside className="admin-sidebar">
        <a className="sidebar-logo" href="/admin">
          <LotusLogo />
          <div className="sidebar-logo-text">
            <div className="sidebar-logo-name">MASSAGE SPA</div>
            <div className="sidebar-logo-tagline">
              Thư giãn · Tái tạo năng lượng
            </div>
          </div>
        </a>

        <nav className="sidebar-nav">
          {NAV_ITEMS.map((item, idx) => (
            <div key={idx}>
              {item.section && (
                <div className="sidebar-section-label">{item.section}</div>
              )}
              <button
                className={`sidebar-item${isActive(item.path) ? " active" : ""}`}
                onClick={() => {
                  if (item.children) {
                    setExpandedMenu((current) => current === item.path ? null : item.path);
                  } else {
                    navigate(item.path);
                  }
                }}
              >
                <span className="sidebar-item-icon">{item.icon}</span>
                <span>{item.label}</span>
                {item.children && <ChevronDown className={`sidebar-item-chevron${expandedMenu === item.path ? " expanded" : ""}`} size={14} />}
                {item.badge ? (
                  <span className="sidebar-item-badge">{item.badge}</span>
                ) : null}
              </button>
              {item.children && expandedMenu === item.path && (
                <div className="sidebar-submenu">
                  {item.children.map((child) => (
                    <button
                      key={child.label}
                      className={`sidebar-submenu-item${location.pathname === child.path.split("?")[0] && !child.path.includes("?") ? " active" : ""}`}
                      onClick={() => navigate(child.path)}
                    >
                      <span />{child.label}
                    </button>
                  ))}
                </div>
              )}
            </div>
          ))}
        </nav>

        <div className="sidebar-footer">
          <span className="sidebar-status-dot" />
          <div className="sidebar-footer-text">
            <div>Hệ thống hoạt động tốt</div>
            <div>v1.0.0</div>
          </div>
        </div>
      </aside>

      {/* MAIN */}
      <div className="admin-main">
        {/* HEADER */}
        <header className="admin-header">
          <div className="header-search">
            <Search className="header-search-icon" size={15} />
            <input
              className="header-search-input"
              placeholder="Tìm kiếm khách hàng, đơn hàng, dịch vụ..."
            />
          </div>

          <div className="header-spacer" />

          <div className="header-actions">
            <button className="header-icon-btn" title="Thông báo">
              <Bell size={17} />
              <span className="header-notif-badge">3</span>
            </button>

            <div
              className="header-user"
              onClick={() => setShowUserMenu((s) => !s)}
              style={{ position: "relative" }}
            >
              <div className="header-avatar">
                {user?.fullName?.charAt(0).toUpperCase() || "A"}
              </div>
              <div>
                <div className="header-user-name">
                  {user?.fullName || "Admin"}
                </div>
                <div className="header-user-role">Quản trị viên</div>
              </div>
              <ChevronDown
                size={14}
                style={{ color: "#94a3b8", marginLeft: 2 }}
              />

              {showUserMenu && (
                <div
                  style={{
                    position: "absolute",
                    top: "110%",
                    right: 0,
                    background: "#fff",
                    border: "1px solid #e2e8f0",
                    borderRadius: 10,
                    boxShadow: "0 8px 24px rgba(0,0,0,0.12)",
                    minWidth: 160,
                    padding: "4px 0",
                    zIndex: 200,
                  }}
                >
                  <button
                    style={{
                      display: "flex",
                      alignItems: "center",
                      gap: 8,
                      width: "100%",
                      padding: "9px 14px",
                      border: "none",
                      background: "none",
                      cursor: "pointer",
                      fontSize: 13,
                      color: "#374151",
                    }}
                    onClick={handleLogout}
                  >
                    <LogOut size={14} />
                    Đăng xuất
                  </button>
                </div>
              )}
            </div>
          </div>
        </header>

        {/* CONTENT */}
        <main className="admin-content">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
