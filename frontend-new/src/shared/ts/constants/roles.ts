export const ROLES = {
  ADMIN: "ADMIN",
  EMPLOYEE: "EMPLOYEE",
  CUSTOMER: "CUSTOMER",
} as const;

export type Role = (typeof ROLES)[keyof typeof ROLES];
