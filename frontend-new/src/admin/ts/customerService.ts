import api from "../../shared/ts/api/api";

export interface CustomerDto {
  id: number;
  userId: number;
  fullName: string;
  email: string;
  phone: string;
  address?: string;
  dateOfBirth?: string;
  gender?: string;
  loyaltyPoints: number;
  isActive: boolean;
  createdAt: string;
}

export interface CustomerPayload {
  fullName: string;
  email?: string;
  phone: string;
  password?: string;
  address?: string;
  dateOfBirth?: string;
  gender?: string;
}

export interface AppointmentHistory {
  id: number;
  serviceName: string;
  appointmentDate: string;
  startTime: string;
  status: string;
}

export interface OrderHistory {
  id: number;
  orderCode: string;
  customerId: number;
  totalAmount: number;
  status: string;
  createdAt: string;
}

export const getCustomers = () => api.get<CustomerDto[]>("/customers").then((r) => r.data);
export const getCustomer = (id: number) => api.get<CustomerDto>(`/customers/${id}`).then((r) => r.data);
export const createCustomer = (payload: Required<Pick<CustomerPayload, "fullName" | "email" | "phone" | "password">> & Omit<CustomerPayload, "email" | "password">) =>
  api.post<CustomerDto>("/customers", payload).then((r) => r.data);
export const updateCustomer = (id: number, payload: Omit<CustomerPayload, "email" | "password">) =>
  api.put<CustomerDto>(`/customers/${id}`, payload).then((r) => r.data);
export const deactivateCustomer = (id: number) => api.delete(`/customers/${id}`).then((r) => r.data);
export const toggleCustomer = (userId: number, isActive: boolean) =>
  api.patch(`/accounts/${userId}/toggle-active`, { isActive }).then((r) => r.data);
export const getAppointmentHistory = (customerId: number) =>
  api.get<AppointmentHistory[]>(`/appointments/customer/${customerId}`).then((r) => r.data);
export const getOrderHistory = () => api.get<OrderHistory[]>("/orders").then((r) => r.data);
