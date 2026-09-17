import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  Tooltip,
  ResponsiveContainer,
  CartesianGrid,
  PieChart,
  Pie,
  Cell,
} from "recharts";
import {
  CalendarPlus,
  ShoppingCart,
  TrendingUp,
  Calendar,
  Package,
  Users,
  AlertTriangle,
  ChevronRight,
  CalendarClock,
} from "lucide-react";
import {
  getOverview,
  getRevenueByDay,
  getAppointmentStats,
  getLowStock,
} from "../../ts/reportService";
import type {
  OverviewStats,
  DailyRevenue,
  LowStockProduct,
} from "../../ts/reportService";
import { getTodayAppointments } from "../../ts/appointmentService";
import type { AppointmentDto } from "../../ts/appointmentService";
import { getAllOrders } from "../../ts/orderService";
import type { OrderDto } from "../../ts/orderService";
import "../../css/dashboard.css";

// ===== Helpers =====
const fmt = (n: number) => new Intl.NumberFormat("vi-VN").format(n) + "đ";

const fmtShort = (n: number) => {
  if (n >= 1_000_000)
    return (n / 1_000_000).toFixed(1).replace(".0", "") + "tr";
  if (n >= 1_000) return (n / 1_000).toFixed(0) + "k";
  return n.toString();
};

const toDateStr = (d: Date) => d.toISOString().slice(0, 10);

const STATUS_LABELS: Record<string, string> = {
  PENDING: "Chờ xác nhận",
  CONFIRMED: "Đã xác nhận",
  CHECKED_IN: "Đã check-in",
  IN_PROGRESS: "Đang thực hiện",
  COMPLETED: "Hoàn thành",
  CANCELLED: "Đã hủy",
};
const STATUS_COLORS: Record<string, string> = {
  PENDING: "#f59e0b",
  CONFIRMED: "#3b82f6",
  CHECKED_IN: "#06b6d4",
  IN_PROGRESS: "#22c55e",
  COMPLETED: "#10b981",
  CANCELLED: "#ef4444",
};
const ORDER_STATUS_LABELS: Record<string, string> = {
  PENDING: "Chờ xử lý",
  CONFIRMED: "Đã xác nhận",
  PREPARING: "Đang chuẩn bị",
  SHIPPING: "Đang giao",
  DELIVERED: "Đã giao",
  COMPLETED: "Hoàn thành",
  CANCELLED: "Đã hủy",
};

// ===== Custom Tooltip for chart =====
const RevenueTooltip = ({
  active,
  payload,
  label,
}: {
  active?: boolean;
  payload?: { value: number }[];
  label?: string;
}) => {
  if (!active || !payload?.length) return null;
  return (
    <div
      style={{
        background: "#0f172a",
        color: "#fff",
        padding: "8px 12px",
        borderRadius: 8,
        fontSize: 12,
      }}
    >
      <div style={{ color: "#94a3b8", marginBottom: 2 }}>{label}</div>
      <div style={{ fontWeight: 700 }}>{fmt(payload[0].value)}</div>
    </div>
  );
};

export default function TongQuanAdmin() {
  const navigate = useNavigate();
  const today = new Date();
  const todayStr = today.toLocaleDateString("vi-VN", {
    weekday: "long",
    year: "numeric",
    month: "long",
    day: "numeric",
  });

  // State
  const [overview, setOverview] = useState<OverviewStats | null>(null);
  const [revenue7, setRevenue7] = useState<DailyRevenue[]>([]);
  const [apptStats, setApptStats] = useState<
    { status: string; count: number }[]
  >([]);
  const [todayAppts, setTodayAppts] = useState<AppointmentDto[]>([]);
  const [newOrders, setNewOrders] = useState<OrderDto[]>([]);
  const [lowStock, setLowStock] = useState<LowStockProduct[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const load = async () => {
      try {
        const to = toDateStr(today);
        const from7 = toDateStr(new Date(today.getTime() - 6 * 86400000));
        const [ov, rev, appt, appts, orders, stock] = await Promise.all([
          getOverview(),
          getRevenueByDay(from7, to),
          getAppointmentStats(to, to),
          getTodayAppointments(),
          getAllOrders({ status: "PENDING" }),
          getLowStock(),
        ]);
        setOverview(ov);
        setRevenue7(rev);
        setApptStats(appt.byStatus || []);
        setTodayAppts(appts);
        setNewOrders(orders.slice(0, 8));
        setLowStock(stock.slice(0, 7));
      } catch {
        setError("Không thể tải dữ liệu. Vui lòng kiểm tra kết nối backend.");
      } finally {
        setLoading(false);
      }
    };
    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  if (loading)
    return (
      <div className="dash-loading">
        <span className="dash-spinner" />
        Đang tải dữ liệu...
      </div>
    );

  if (error)
    return (
      <div className="dash-loading" style={{ color: "#ef4444" }}>
        <AlertTriangle size={18} />
        {error}
      </div>
    );

  // Chart data — format label
  const chartData = revenue7.map((d) => ({
    date: d.date.slice(5).replace("-", "/"), // MM/DD
    revenue: d.revenue,
  }));

  // Pie data
  const totalAppts = apptStats.reduce((s, x) => s + x.count, 0);
  const pieData = apptStats.map((a) => ({
    name: STATUS_LABELS[a.status] || a.status,
    value: a.count,
    color: STATUS_COLORS[a.status] || "#94a3b8",
    status: a.status,
  }));

  return (
    <div>
      {/* Page header */}
      <div className="dash-page-header">
        <div>
          <h1 className="dash-page-title">Tổng quan</h1>
          <p className="dash-page-sub">
            Chào mừng bạn trở lại! Đây là tình hình hoạt động của hệ thống
            Massage Spa.
          </p>
        </div>
        <div className="dash-header-actions">
          <button
            className="dash-btn dash-btn--primary"
            onClick={() => navigate("/admin/lich-hen/tao")}
          >
            <CalendarPlus size={15} />
            Đặt lịch mới
          </button>
          <button
            className="dash-btn dash-btn--secondary"
            onClick={() => navigate("/admin/don-hang/tao")}
          >
            <ShoppingCart size={15} />
            Tạo đơn hàng
          </button>
        </div>
      </div>

      {/* Date */}
      <div className="dash-date">
        <CalendarClock size={14} />
        <span>Hôm nay: {todayStr}</span>
      </div>

      {/* Stat cards */}
      <div className="dash-stats">
        <div className="stat-card">
          <div className="stat-icon stat-icon--green">
            <TrendingUp size={22} />
          </div>
          <div className="stat-body">
            <div className="stat-label">Doanh thu hôm nay</div>
            <div className="stat-value">{fmt(overview?.todayRevenue ?? 0)}</div>
            <div className="stat-change stat-change--up">
              <TrendingUp size={12} />
              <span>Từ đơn hoàn thành</span>
            </div>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon stat-icon--blue">
            <Calendar size={22} />
          </div>
          <div className="stat-body">
            <div className="stat-label">Lịch hẹn hôm nay</div>
            <div className="stat-value">
              {overview?.todayAppointments ?? todayAppts.length}
            </div>
            <div className="stat-change stat-change--up">
              <TrendingUp size={12} />
              <span>{overview?.pendingAppointments ?? 0} chờ xác nhận</span>
            </div>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon stat-icon--purple">
            <ShoppingCart size={22} />
          </div>
          <div className="stat-body">
            <div className="stat-label">Đơn hàng</div>
            <div className="stat-value">{overview?.todayOrders ?? 0}</div>
            <div className="stat-change stat-change--up">
              <TrendingUp size={12} />
              <span>{overview?.pendingOrders ?? 0} đơn chờ xử lý</span>
            </div>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon stat-icon--orange">
            <Users size={22} />
          </div>
          <div className="stat-body">
            <div className="stat-label">Khách hàng</div>
            <div className="stat-value">
              {overview?.totalCustomers?.toLocaleString("vi-VN") ?? 0}
            </div>
            <div className="stat-change stat-change--up">
              <TrendingUp size={12} />
              <span>Tổng khách hàng</span>
            </div>
          </div>
        </div>
      </div>

      {/* Charts row */}
      <div className="dash-charts">
        {/* Line chart */}
        <div className="dash-card">
          <div className="dash-card-header">
            <div>
              <div className="dash-card-title">Doanh thu 7 ngày gần đây</div>
              <div className="dash-card-sub">Đơn vị: VNĐ</div>
            </div>
          </div>
          <div className="dash-card-body" style={{ paddingTop: 8 }}>
            {chartData.length === 0 ? (
              <div
                style={{
                  textAlign: "center",
                  padding: "40px",
                  color: "#94a3b8",
                  fontSize: 13,
                }}
              >
                Chưa có dữ liệu doanh thu
              </div>
            ) : (
              <ResponsiveContainer width="100%" height={220}>
                <LineChart
                  data={chartData}
                  margin={{ top: 8, right: 12, left: 0, bottom: 0 }}
                >
                  <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" />
                  <XAxis
                    dataKey="date"
                    tick={{ fontSize: 11, fill: "#94a3b8" }}
                    axisLine={false}
                    tickLine={false}
                  />
                  <YAxis
                    tickFormatter={fmtShort}
                    tick={{ fontSize: 10, fill: "#94a3b8" }}
                    axisLine={false}
                    tickLine={false}
                    width={50}
                  />
                  <Tooltip content={<RevenueTooltip />} />
                  <Line
                    type="monotone"
                    dataKey="revenue"
                    stroke="#2563eb"
                    strokeWidth={2.5}
                    dot={{ fill: "#2563eb", r: 4 }}
                    activeDot={{ r: 6, fill: "#2563eb" }}
                  />
                </LineChart>
              </ResponsiveContainer>
            )}
          </div>
        </div>

        {/* Donut chart */}
        <div className="dash-card">
          <div className="dash-card-header">
            <div>
              <div className="dash-card-title">Trạng thái lịch hẹn hôm nay</div>
            </div>
          </div>
          <div className="dash-card-body">
            {pieData.length === 0 ? (
              <div
                style={{
                  textAlign: "center",
                  padding: "40px 20px",
                  color: "#94a3b8",
                  fontSize: 13,
                }}
              >
                Không có lịch hẹn hôm nay
              </div>
            ) : (
              <>
                <div style={{ position: "relative" }}>
                  <ResponsiveContainer width="100%" height={180}>
                    <PieChart>
                      <Pie
                        data={pieData}
                        cx="50%"
                        cy="50%"
                        innerRadius={52}
                        outerRadius={80}
                        dataKey="value"
                        paddingAngle={2}
                      >
                        {pieData.map((entry, i) => (
                          <Cell key={i} fill={entry.color} />
                        ))}
                      </Pie>
                      <Tooltip formatter={(v) => [`${v} lịch`, ""]} />
                    </PieChart>
                  </ResponsiveContainer>
                  <div
                    style={{
                      position: "absolute",
                      top: "50%",
                      left: "50%",
                      transform: "translate(-50%, -50%)",
                      textAlign: "center",
                      pointerEvents: "none",
                    }}
                  >
                    <div
                      style={{
                        fontSize: 22,
                        fontWeight: 800,
                        color: "#0f172a",
                      }}
                    >
                      {totalAppts}
                    </div>
                    <div style={{ fontSize: 11, color: "#64748b" }}>
                      lịch hẹn
                    </div>
                  </div>
                </div>
                <div className="donut-legend">
                  {pieData.map((item, i) => (
                    <div className="donut-legend-item" key={i}>
                      <span
                        className="donut-dot"
                        style={{ background: item.color }}
                      />
                      <span>{item.name}</span>
                      <span className="donut-legend-count">{item.value}</span>
                      <span className="donut-legend-pct">
                        {totalAppts > 0
                          ? ((item.value / totalAppts) * 100).toFixed(1)
                          : 0}
                        %
                      </span>
                    </div>
                  ))}
                </div>
              </>
            )}
          </div>
        </div>
      </div>

      {/* Bottom section */}
      <div className="dash-bottom">
        {/* Today appointments */}
        <div className="dash-card">
          <div className="dash-card-header">
            <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
              <CalendarClock size={16} color="#2563eb" />
              <span className="dash-card-title">Lịch hẹn hôm nay</span>
            </div>
            <button
              className="dash-view-all"
              onClick={() => navigate("/admin/lich-hen")}
            >
              Xem tất cả <ChevronRight size={13} />
            </button>
          </div>
          <div className="dash-card-body" style={{ padding: "8px 0 0" }}>
            <div className="dash-table-wrap">
              <table className="dash-table">
                <thead>
                  <tr>
                    <th>#</th>
                    <th>Khách hàng</th>
                    <th>Dịch vụ</th>
                    <th>Giờ</th>
                    <th>Phòng</th>
                    <th>Nhân viên</th>
                    <th>Trạng thái</th>
                  </tr>
                </thead>
                <tbody>
                  {todayAppts.length === 0 ? (
                    <tr>
                      <td
                        colSpan={7}
                        style={{
                          textAlign: "center",
                          color: "#94a3b8",
                          padding: "20px",
                        }}
                      >
                        Không có lịch hẹn hôm nay
                      </td>
                    </tr>
                  ) : (
                    todayAppts.slice(0, 8).map((a, i) => (
                      <tr key={a.id}>
                        <td style={{ color: "#94a3b8" }}>{i + 1}</td>
                        <td style={{ fontWeight: 600 }}>{a.customerName}</td>
                        <td>{a.serviceName}</td>
                        <td>{a.startTime?.slice(0, 5)}</td>
                        <td>{a.roomNumber ?? "—"}</td>
                        <td>{a.employeeName ?? "—"}</td>
                        <td>
                          <span className={`status-badge status-${a.status}`}>
                            {STATUS_LABELS[a.status] || a.status}
                          </span>
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          </div>
        </div>

        {/* New orders */}
        <div className="dash-card">
          <div className="dash-card-header">
            <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
              <ShoppingCart size={16} color="#9333ea" />
              <span className="dash-card-title">Đơn hàng mới</span>
            </div>
            <button
              className="dash-view-all"
              onClick={() => navigate("/admin/don-hang")}
            >
              Xem tất cả <ChevronRight size={13} />
            </button>
          </div>
          <div className="dash-card-body" style={{ padding: "8px 0 0" }}>
            <div className="dash-table-wrap">
              <table className="dash-table">
                <thead>
                  <tr>
                    <th>#</th>
                    <th>Mã đơn</th>
                    <th>Khách hàng</th>
                    <th>Tổng tiền</th>
                    <th>Trạng thái</th>
                    <th>Thời gian</th>
                  </tr>
                </thead>
                <tbody>
                  {newOrders.length === 0 ? (
                    <tr>
                      <td
                        colSpan={6}
                        style={{
                          textAlign: "center",
                          color: "#94a3b8",
                          padding: "20px",
                        }}
                      >
                        Không có đơn hàng mới
                      </td>
                    </tr>
                  ) : (
                    newOrders.map((o, i) => (
                      <tr key={o.id}>
                        <td style={{ color: "#94a3b8" }}>{i + 1}</td>
                        <td style={{ fontWeight: 600, color: "#2563eb" }}>
                          {o.orderCode}
                        </td>
                        <td>{o.customerName}</td>
                        <td style={{ fontWeight: 600 }}>
                          {fmt(o.totalAmount)}
                        </td>
                        <td>
                          <span className={`status-badge status-${o.status}`}>
                            {ORDER_STATUS_LABELS[o.status] || o.status}
                          </span>
                        </td>
                        <td style={{ color: "#94a3b8" }}>
                          {new Date(o.createdAt).toLocaleString("vi-VN", {
                            hour: "2-digit",
                            minute: "2-digit",
                            day: "2-digit",
                            month: "2-digit",
                          })}
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          </div>
        </div>

        {/* Low stock */}
        <div className="dash-card">
          <div className="dash-card-header">
            <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
              <AlertTriangle size={16} color="#f59e0b" />
              <span className="dash-card-title">Cảnh báo tồn kho thấp</span>
            </div>
            <button
              className="dash-view-all"
              onClick={() => navigate("/admin/kho")}
            >
              Xem tất cả <ChevronRight size={13} />
            </button>
          </div>
          <div className="dash-card-body">
            {lowStock.length === 0 ? (
              <div
                style={{
                  textAlign: "center",
                  padding: "30px 16px",
                  color: "#94a3b8",
                  fontSize: 13,
                }}
              >
                Không có sản phẩm tồn kho thấp
              </div>
            ) : (
              <div className="low-stock-list">
                {lowStock.map((p) => (
                  <div className="low-stock-item" key={p.id}>
                    <div className="low-stock-img">
                      <Package size={16} />
                    </div>
                    <div className="low-stock-info">
                      <div className="low-stock-name">{p.name}</div>
                      <div className="low-stock-qty">
                        Còn: <strong>{p.stockQuantity}</strong>
                      </div>
                    </div>
                    <span
                      className={`low-stock-alert${p.stockQuantity <= 5 ? " low-stock-alert--critical" : ""}`}
                    >
                      ≤{p.stockQuantity <= 5 ? "5" : "10"}
                    </span>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
