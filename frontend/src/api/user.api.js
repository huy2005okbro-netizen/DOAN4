import axios from "./axios";

export const getAllUsersAPI = () => axios.get("/users");
export const getUserByIdAPI = (id) => axios.get(`/users/${id}`);
export const updateUserAPI = (id, data) => axios.put(`/users/${id}`, data);
export const deleteUserAPI = (id) => axios.delete(`/users/${id}`);
