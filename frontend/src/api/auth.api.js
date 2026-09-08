import axios from "./axios";

export const loginAPI = (data) => axios.post("/auth/login", data);
export const registerAPI = (data) => axios.post("/auth/register", data);
export const getMeAPI = () => axios.get("/auth/me");
