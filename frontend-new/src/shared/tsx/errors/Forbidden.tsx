import { Link } from "react-router-dom";

export default function Forbidden() {
  return (
    <div style={{ textAlign: "center", padding: "4rem" }}>
      <h1>403</h1>
      <p>Bạn không có quyền truy cập trang này.</p>
      <Link to="/">Về trang chủ</Link>
    </div>
  );
}
