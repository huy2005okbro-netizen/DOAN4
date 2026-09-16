import api from "../../shared/ts/api/api";

export interface WorkSchedule {
  id: number;
  date: string;
  shiftStart: string;
  shiftEnd: string;
  status: string;
}

export const getMySchedule = (month: number, year: number) =>
  api
    .get<WorkSchedule[]>(`/employees/me/schedule?month=${month}&year=${year}`)
    .then((r) => r.data);
