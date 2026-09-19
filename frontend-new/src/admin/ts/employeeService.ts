import api from "../../shared/ts/api/api";

export type Position = "RECEPTIONIST" | "MASSAGE_THERAPIST";
export interface EmployeeDto { id:number; userId:number; fullName:string; email:string; phone:string; position?:string; startDate:string; isActive:boolean; createdAt:string }
export interface EmployeePayload { fullName:string; email?:string; phone:string; password?:string; position:string; startDate:string; isActive?:boolean }
export interface AssignedAppointment { id:number; customerName:string; serviceName:string; appointmentDate:string; startTime:string; endTime:string; status:string; employeeId?:number }

export const getEmployees=()=>api.get<EmployeeDto[]>("/employees").then(r=>r.data);
export const createEmployee=(data:Required<Pick<EmployeePayload,"fullName"|"email"|"phone"|"password"|"position"|"startDate">>)=>api.post<EmployeeDto>("/employees",data).then(r=>r.data);
export const updateEmployee=(id:number,data:Partial<Omit<EmployeePayload,"email"|"password">>)=>api.put<EmployeeDto>(`/employees/${id}`,data).then(r=>r.data);
export const removeEmployee=(id:number)=>api.delete(`/employees/${id}`).then(r=>r.data);
export const getAppointments=()=>api.get<AssignedAppointment[]>("/appointments").then(r=>r.data);
