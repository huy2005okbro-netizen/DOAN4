import api from "../../shared/ts/api/api";

export interface TherapistCustomer {
  id: number;
  fullName: string;
  phone: string;
  totalSessions: number;
  lastVisit: string;
  notes?: string;
}

export const getMyCustomers = () =>
  api.get<TherapistCustomer[]>("/employees/me/customers").then((r) => r.data);
