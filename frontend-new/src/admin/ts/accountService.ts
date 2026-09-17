import api from "../../shared/ts/api/api";

export interface AccountDto {
  userId: number;
  fullName: string;
  email: string;
  phone: string;
  role: "ADMIN" | "EMPLOYEE" | "CUSTOMER";
  isActive: boolean;
  createdAt: string;
  // Employee
  employeeId?: number;
  position?: string;
  startDate?: string;
  // Customer
  customerId?: number;
  loyaltyPoints?: number;
  address?: string;
  dateOfBirth?: string;
  gender?: string;
}

export interface AccountStats {
  total: number;
  admins: number;
  employees: number;
  customers: number;
  active: number;
  locked: number;
}

export interface CreateEmployeePayload {
  fullName: string;
  email: string;
  phone: string;
  password: string;
  position?: string;
  startDate: string;
}

export interface CreateCustomerPayload {
  fullName: string;
  email: string;
  phone: string;
  password: string;
  address?: string;
  dateOfBirth?: string;
  gender?: string;
}

export interface UpdateEmployeePayload {
  fullName?: string;
  phone?: string;
  position?: string;
  isActive?: boolean;
}

export interface UpdateCustomerPayload {
  fullName?: string;
  phone?: string;
  address?: string;
  dateOfBirth?: string;
  gender?: string;
  isActive?: boolean;
}

export interface UpdateAdminPayload {
  fullName?: string;
  phone?: string;
}

export const POSITION_OPTIONS = [
  { value: "RECEPTIONIST", label: "Lễ tân" },
  { value: "MASSAGE_THERAPIST", label: "Kỹ thuật viên Massage" },
];

// Lấy danh sách tài khoản (unified)
export const getAccounts = (params?: {
  role?: string;
  isActive?: boolean;
  search?: string;
}) => api.get<AccountDto[]>("/accounts", { params }).then((r) => r.data);

// Thống kê
export const getAccountStats = () =>
  api.get<AccountStats>("/accounts/stats").then((r) => r.data);

// Chi tiết
export const getAccountById = (userId: number) =>
  api.get<AccountDto>(`/accounts/${userId}`).then((r) => r.data);

// Tạo nhân viên
export const createEmployee = (payload: CreateEmployeePayload) =>
  api.post<AccountDto>("/accounts/employee", payload).then((r) => r.data);

// Tạo khách hàng
export const createCustomer = (payload: CreateCustomerPayload) =>
  api.post<AccountDto>("/accounts/customer", payload).then((r) => r.data);

// Cập nhật nhân viên
export const updateEmployee = (
  employeeId: number,
  payload: UpdateEmployeePayload,
) =>
  api
    .put<AccountDto>(`/accounts/employee/${employeeId}`, payload)
    .then((r) => r.data);

// Cập nhật khách hàng
export const updateCustomer = (
  customerId: number,
  payload: UpdateCustomerPayload,
) =>
  api
    .put<AccountDto>(`/accounts/customer/${customerId}`, payload)
    .then((r) => r.data);

export const updateAdmin = (userId: number, payload: UpdateAdminPayload) =>
  api.put<AccountDto>(`/accounts/admin/${userId}`, payload).then((r) => r.data);

// Khóa / mở khóa
export const toggleActive = (userId: number, isActive: boolean) =>
  api
    .patch(`/accounts/${userId}/toggle-active`, { isActive })
    .then((r) => r.data);

// Admin reset mật khẩu
export const adminResetPassword = (userId: number, newPassword: string) =>
  api
    .post(`/accounts/${userId}/reset-password`, { newPassword })
    .then((r) => r.data);
