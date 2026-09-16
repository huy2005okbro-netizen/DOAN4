import api from "../../shared/ts/api/api";

export interface TherapistProfile {
  id: number;
  fullName: string;
  email: string;
  phone: string;
  position: string;
  startDate: string;
  avatar?: string;
}

export const getMyProfile = () =>
  api.get<TherapistProfile>("/employees/me").then((r) => r.data);

export const updateMyProfile = (data: Partial<TherapistProfile>) =>
  api.put("/employees/me", data).then((r) => r.data);

export const changePassword = (data: {
  currentPassword: string;
  newPassword: string;
}) => api.post("/auth/change-password", data).then((r) => r.data);
