const TOKEN_KEY = "massage_token";
const USER_KEY = "massage_user";

export const getToken = (): string | null => localStorage.getItem(TOKEN_KEY);
export const setToken = (token: string): void =>
  localStorage.setItem(TOKEN_KEY, token);
export const removeToken = (): void => localStorage.removeItem(TOKEN_KEY);

export const getUser = <T>(): T | null => {
  const raw = localStorage.getItem(USER_KEY);
  return raw ? (JSON.parse(raw) as T) : null;
};
export const setUser = (user: unknown): void =>
  localStorage.setItem(USER_KEY, JSON.stringify(user));
export const removeUser = (): void => localStorage.removeItem(USER_KEY);

export const clearAuth = (): void => {
  removeToken();
  removeUser();
};
