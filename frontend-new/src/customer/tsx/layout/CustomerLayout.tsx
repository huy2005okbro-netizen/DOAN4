import { Link, Outlet, useNavigate } from "react-router-dom";
import { LogOut, Menu, Sparkles, UserRound, X } from "lucide-react";
import { useState } from "react";
import { useAuth } from "../../../shared/ts/hooks/useAuth";
import "../../css/layout.css";

export default function CustomerLayout() {
  const [open,setOpen]=useState(false); const { user,logout }=useAuth(); const navigate=useNavigate();
  return <div className="customer-layout"><header className="customer-header"><Link to="/customer" className="customer-logo"><Sparkles/> <span>MASSAGE SPA<small>Thư giãn · Tái tạo năng lượng</small></span></Link><button className="customer-menu" onClick={()=>setOpen(!open)}>{open?<X/>:<Menu/>}</button><nav className={open?"customer-nav open":"customer-nav"}><Link to="/customer">Trang chủ</Link><Link to="/customer/dich-vu">Dịch vụ</Link><Link to="/customer/san-pham">Sản phẩm</Link><Link to="/customer/lich-hen">Lịch hẹn của tôi</Link></nav><div className="customer-user"><UserRound/><span>{user?.fullName||"Khách hàng"}<small>Khách hàng</small></span><button title="Chọn lại khu vực đăng nhập" onClick={()=>{logout();navigate("/",{replace:true})}}><LogOut size={18}/></button></div></header><main><Outlet/></main><footer className="customer-footer"><div><Sparkles/> <b>MASSAGE SPA</b><p>Chăm sóc cơ thể, nuôi dưỡng tâm hồn.</p></div><div><b>Liên kết nhanh</b><Link to="/customer/dich-vu">Dịch vụ massage</Link><Link to="/customer/lich-hen">Lịch hẹn của tôi</Link></div><div><b>Liên hệ</b><p>Hotline: 0900 000 000</p><p>08:00 – 21:00 hằng ngày</p></div></footer></div>;
}
