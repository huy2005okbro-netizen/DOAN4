import { useEffect, useMemo, useState } from "react";
import { CalendarDays, ChevronLeft, ChevronRight, Search, Users, X } from "lucide-react";
import api from "../../../shared/ts/api/api";
import { getEmployees } from "../../ts/employeeService";
import type { EmployeeDto } from "../../ts/employeeService";
import "../../css/khach-hang.css";

type Shift = { id:number; employeeId:number; employeeName:string; position?:string; workDate:string; shiftType:string; startTime:string; endTime:string; status:string; note?:string };
type Slot = { key:string; date:string; shiftType:"MORNING"|"AFTERNOON"|"EVENING"; start:string; end:string; rows:Shift[] };

const defaults = [
  { shiftType:"MORNING" as const, name:"CA SÁNG", start:"08:00", end:"14:00" },
  { shiftType:"AFTERNOON" as const, name:"CA CHIỀU", start:"14:00", end:"18:00" },
  { shiftType:"EVENING" as const, name:"CA TỐI", start:"18:00", end:"22:00" },
];
const position = (value?:string) => value === "MASSAGE_THERAPIST" ? "Kỹ thuật viên" : value === "RECEPTIONIST" ? "Lễ tân" : value || "Nhân viên";
const localDate = (date:Date) => `${date.getFullYear()}-${String(date.getMonth()+1).padStart(2,"0")}-${String(date.getDate()).padStart(2,"0")}`;
const mondayOf = (date:Date) => { const d=new Date(date); d.setHours(0,0,0,0); d.setDate(d.getDate()-(d.getDay()+6)%7); return d; };
const displayDate=(date:string)=>new Date(`${date}T00:00:00`).toLocaleDateString("vi-VN");
const time=(value:string)=>value.slice(0,5);

export default function PhanCaLamViec() {
  const [week, setWeek] = useState(()=>mondayOf(new Date()));
  const [employees, setEmployees] = useState<EmployeeDto[]>([]);
  const [shifts, setShifts] = useState<Shift[]>([]);
  const [selected, setSelected] = useState<Slot|null>(null);
  const [checked, setChecked] = useState<number[]>([]);
  const [query, setQuery] = useState("");
  const [message, setMessage] = useState("");
  const [saving, setSaving] = useState(false);
  const [copyConfirm, setCopyConfirm] = useState(false);
  const days = useMemo(()=>Array.from({length:7},(_,index)=>{const d=new Date(week);d.setDate(d.getDate()+index);return d;}),[week]);

  const load = async () => {
    try {
      setMessage(""); const end=new Date(week); end.setDate(end.getDate()+6);
      const [activeEmployees, weeklyShifts] = await Promise.all([
        getEmployees(),
        api.get<Shift[]>("/work-shifts",{params:{from:localDate(week),to:localDate(end)}}).then(r=>r.data),
      ]);
      setEmployees(activeEmployees.filter(x=>x.isActive)); setShifts(weeklyShifts);
    } catch { setMessage("Không tải được lịch phân ca. Hãy kiểm tra API WorkShifts và đăng nhập Admin."); }
  };
  useEffect(()=>{load();},[week]);

  // Exactly 21 visual slots are produced regardless of whether a WorkShift row exists.
  const slots = useMemo<Slot[]>(()=>days.flatMap(day=>defaults.map(def=>{
    const date=localDate(day);
    const rows=shifts.filter(x=>x.workDate.slice(0,10)===date && x.shiftType===def.shiftType && time(x.startTime)===def.start && time(x.endTime)===def.end);
    return {key:`${date}|${def.shiftType}`,date,shiftType:def.shiftType,start:def.start,end:def.end,rows};
  })),[days,shifts]);

  const open = (slot:Slot) => { setSelected(slot); setChecked(slot.rows.map(x=>x.employeeId)); setQuery(""); };
  const toggle=(id:number)=>setChecked(current=>current.includes(id)?current.filter(x=>x!==id):[...current,id]);
  const save = async () => {
    if(!selected) return;
    try {
      setSaving(true); setMessage("");
      const body={ids:selected.rows.map(x=>x.id),employeeIds:checked,workDate:selected.date,shiftType:selected.shiftType,startTime:`${selected.start}:00`,endTime:`${selected.end}:00`,status:"WORKING",note:""};
      // An empty default slot is created only when employees are selected.
      if(selected.rows.length) await api.put("/work-shifts/group",body);
      else if(checked.length) await api.post("/work-shifts/bulk",body);
      setSelected(null); await load();
    } catch(error:unknown) { setMessage((error as {response?:{data?:{message?:string}}}).response?.data?.message||"Không thể lưu phân ca."); }
    finally { setSaving(false); }
  };
  const copyPreviousWeek = async () => {
    try {
      setSaving(true); setMessage("");
      const result = await api.post<{message:string}>("/work-shifts/copy-previous-week",null,{params:{weekStart:localDate(week)}}).then(r=>r.data);
      setCopyConfirm(false); setMessage(result.message); await load();
    } catch(error:unknown) { setMessage((error as {response?:{data?:{message?:string}}}).response?.data?.message||"Không thể sao chép lịch tuần trước."); }
    finally { setSaving(false); }
  };
  const visibleEmployees=employees.filter(employee=>[employee.fullName,employee.phone,employee.email,position(employee.position)].join(" ").toLocaleLowerCase().includes(query.toLocaleLowerCase()));

  return <div className="kh-page shift-page">
    <div className="kh-heading"><div><h1><CalendarDays/> Phân ca làm việc</h1><p>Quản lý lịch làm việc của nhân viên theo tuần</p></div></div>
    {message&&<p className="kh-error">{message}</p>}
    <section className="shift-toolbar">
      <div className="shift-nav"><button onClick={()=>setWeek(current=>{const d=new Date(current);d.setDate(d.getDate()-7);return d;})}><ChevronLeft/> Tuần trước</button><button onClick={()=>setWeek(mondayOf(new Date()))}>Tuần hiện tại</button><button onClick={()=>setWeek(current=>{const d=new Date(current);d.setDate(d.getDate()+7);return d;})}>Tuần sau <ChevronRight/></button><button onClick={()=>setCopyConfirm(true)}>Sao chép tuần trước</button></div>
      <strong><CalendarDays size={17}/>{displayDate(localDate(days[0]))} - {displayDate(localDate(days[6]))}</strong>
      <div className="shift-legend"><i className="morning"/>Ca sáng 08:00–14:00<i className="afternoon"/>Ca chiều 14:00–18:00<i className="evening"/>Ca tối 18:00–22:00</div>
    </section>
    <section className="shift-calendar">{days.map((day,index)=>{const date=localDate(day);return <article className="shift-day" key={date}><header><b>{index===6?"Chủ nhật":`Thứ ${index+2}`}</b><small>{displayDate(date)}</small></header>{slots.filter(slot=>slot.date===date).map(slot=>{const def=defaults.find(x=>x.shiftType===slot.shiftType)!;return <button type="button" className={`shift-card ${slot.shiftType.toLowerCase()} ${slot.rows.length?"assigned":""}`} key={slot.key} onClick={()=>open(slot)}><div className="shift-card-title"><strong>{def.name}</strong></div><b>{slot.start} - {slot.end}</b>{slot.rows.length?<div className="shift-people">{slot.rows.slice(0,3).map(row=><span key={row.id}>{row.employeeName}<small>{position(row.position)}</small></span>)}{slot.rows.length>3&&<em>+{slot.rows.length-3} nhân viên</em>}</div>:<div className="shift-empty"><span>Chưa phân nhân viên</span><small>Click để phân ca</small></div>}<small>{slot.rows.length?`${slot.rows.length} nhân viên`:""}</small></button>})}</article>})}</section>
    {selected&&<div className="kh-overlay" onClick={()=>setSelected(null)}><section className="shift-drawer" onClick={event=>event.stopPropagation()}><header><div><h2><Users/> Phân nhân viên vào ca</h2><p>{displayDate(selected.date)} · {defaults.find(x=>x.shiftType===selected.shiftType)?.name} · {selected.start} - {selected.end}</p></div><button onClick={()=>setSelected(null)}><X/></button></header><div className="shift-drawer-body"><div className="shift-select-title"><b>Chọn nhân viên đang hoạt động</b><span>Đã chọn {checked.length} nhân viên</span></div><div className="shift-search"><Search size={17}/><input autoFocus value={query} onChange={event=>setQuery(event.target.value)} placeholder="Tìm kiếm nhân viên..."/></div><div className="shift-employee-list">{visibleEmployees.map(employee=><label key={employee.id}><input type="checkbox" checked={checked.includes(employee.id)} onChange={()=>toggle(employee.id)}/><span><b>{employee.fullName}</b><small>{position(employee.position)}</small></span><em>{position(employee.position)}</em></label>)}{!visibleEmployees.length&&<p>Không tìm thấy nhân viên phù hợp.</p>}</div></div><footer><button className="kh-btn kh-btn-outline" onClick={()=>setSelected(null)}>Hủy</button><button className="kh-btn kh-btn-primary" disabled={saving} onClick={save}>{saving?"Đang lưu...":"Lưu phân ca"}</button></footer></section></div>}
    {copyConfirm&&<div className="kh-overlay"><section className="kh-confirm"><h3>Sao chép tuần trước?</h3><p>Lịch phân ca từ tuần {displayDate(localDate(new Date(week.getTime()-7*86400000)))} - {displayDate(localDate(new Date(week.getTime()-86400000)))} sẽ được sao chép sang tuần đang xem. Các phân công trùng hoặc chồng giờ sẽ được bỏ qua.</p><div><button className="kh-btn kh-btn-outline" onClick={()=>setCopyConfirm(false)}>Hủy</button><button className="kh-btn kh-btn-primary" disabled={saving} onClick={copyPreviousWeek}>Xác nhận sao chép</button></div></section></div>}
  </div>;
}
