export const ROLES = {
  ADMIN: "Admin",
  EMPLOYEE: "Employee",
  CUSTOMER: "Customer",
} as const;

export type Role = (typeof ROLES)[keyof typeof ROLES];
