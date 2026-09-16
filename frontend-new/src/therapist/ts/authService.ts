import api from "../../shared/ts/api/api";
import type { AuthResponse, LoginRequest } from "../../shared/ts/types/auth";

export const loginKyThuatVien = (data: LoginRequest) =>
  api.post<AuthResponse>("/auth/login", data).then((r) => r.data);
