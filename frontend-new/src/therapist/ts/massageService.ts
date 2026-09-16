import api from "../../shared/ts/api/api";

export interface MassageAppointment {
  id: number;
  customerName: string;
  serviceName: string;
  roomName: string;
  scheduledAt: string;
  duration: number;
  status: string;
  notes?: string;
}

export const getMyAppointments = (params?: {
  status?: string;
  date?: string;
}) =>
  api
    .get<MassageAppointment[]>("/appointments/me", { params })
    .then((r) => r.data);

export const getAppointmentDetail = (id: number) =>
  api.get<MassageAppointment>(`/appointments/${id}`).then((r) => r.data);

export const startService = (id: number) =>
  api.patch(`/appointments/${id}/start`).then((r) => r.data);

export const completeService = (id: number, notes?: string) =>
  api.patch(`/appointments/${id}/complete`, { notes }).then((r) => r.data);
