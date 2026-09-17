import api from "../../shared/ts/api/api";

export interface OverviewStats {
  totalCustomers: number;
  totalEmployees: number;
  totalProducts: number;
  totalServices: number;
  todayOrders: number;
  todayRevenue: number;
  todayAppointments: number;
  pendingAppointments: number;
  pendingOrders: number;
}

export interface DailyRevenue {
  date: string;
  revenue: number;
  orderCount: number;
}

export interface AppointmentStatusStat {
  status: string;
  count: number;
}

export interface LowStockProduct {
  id: number;
  name: string;
  categoryName: string;
  stockQuantity: number;
  price: number;
  brand: string;
}

export const getOverview = () =>
  api.get<OverviewStats>("/reports/overview").then((r) => r.data);

export const getRevenueByDay = (from: string, to: string) =>
  api
    .get<DailyRevenue[]>(`/reports/revenue-by-day?from=${from}&to=${to}`)
    .then((r) => r.data);

export const getAppointmentStats = (from: string, to: string) =>
  api
    .get<{
      from: string;
      to: string;
      byStatus: AppointmentStatusStat[];
    }>(`/reports/appointments?from=${from}&to=${to}`)
    .then((r) => r.data);

export const getLowStock = () =>
  api
    .get<LowStockProduct[]>("/reports/inventory?lowStockOnly=true")
    .then((r) => r.data);
