import axios from "axios";
import { getToken, removeToken } from "../utils/storage";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || "/api",
  headers: {
    "Content-Type": "application/json",
  },
});

// Request interceptor – attach JWT
api.interceptors.request.use((config) => {
  const token = getToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Response interceptor – handle 401
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Chỉ redirect nếu đang ở trang cần auth, không phải trang login
      const path = window.location.pathname;
      const isAuthPage =
        path.includes("dang-nhap") || path.includes("dang-ky") || path === "/";
      if (!isAuthPage) {
        removeToken();
        window.location.href = "/";
      }
    }
    return Promise.reject(error);
  },
);

export default api;
