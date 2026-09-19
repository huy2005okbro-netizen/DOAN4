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
export interface OrderDetailDto extends OrderDto { subTotal:number; discountAmount:number; shippingFee:number; shippingAddress?:string; note?:string; items:{id:number;productName:string;quantity:number;unitPrice:number;totalPrice:number}[] }

export const getAllOrders = (params?: { status?: string }) =>
  api.get<OrderDto[]>("/orders", { params }).then((r) => r.data);
export const getOrder = (id:number) => api.get<OrderDetailDto>(`/orders/${id}`).then(r=>r.data);
