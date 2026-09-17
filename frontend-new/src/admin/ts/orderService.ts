import api from "../../shared/ts/api/api";

export type OrderStatus =
  | "PENDING"
  | "CONFIRMED"
  | "PREPARING"
  | "SHIPPING"
  | "DELIVERED"
  | "COMPLETED"
  | "CANCELLED";

export interface OrderDto {
  id: number;
  orderCode: string;
  customerId: number;
  customerName: string;
  totalAmount: number;
  status: OrderStatus;
  orderType: string;
  createdAt: string;
}

export const getAllOrders = (params?: { status?: string }) =>
  api.get<OrderDto[]>("/orders", { params }).then((r) => r.data);
