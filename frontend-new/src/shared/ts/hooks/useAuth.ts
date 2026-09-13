import { useContext } from "react";
import { AuthContext } from "../../../shared/tsx/context/AuthContext";

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used within AuthProvider");
  return context;
};
