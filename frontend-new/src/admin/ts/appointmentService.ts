import api from "../../shared/ts/api/api";

export type AppointmentStatus =
  | "PENDING"
  | "CONFIRMED"
  | "CHECKED_IN"
  | "IN_PROGRESS"
  | "COMPLETED"
  | "CANCELLED";

export interface AppointmentDto {
  id: number;
  customerId: number;
  customerName: string;
  serviceId: number;
  serviceName: string;
  servicePrice: number;
  employeeId?: number;
  employeeName?: string;
  roomId?: number;
  roomNumber?: string;
  appointmentDate: string;
  startTime: string;
  endTime: string;
  status: AppointmentStatus;
  note?: string;
  createdAt: string;
}

export const getTodayAppointments = () =>
  api.get<AppointmentDto[]>("/appointments/today").then((r) => r.data);

export const getAllAppointments = (params?: {
  status?: string;
  date?: string;
}) =>
  api.get<AppointmentDto[]>("/appointments", { params }).then((r) => r.data);
