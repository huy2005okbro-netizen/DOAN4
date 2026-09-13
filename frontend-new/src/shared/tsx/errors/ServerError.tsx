import { Link } from "react-router-dom";

export default function ServerError() {
  return (
    <div style={{ textAlign: "center", padding: "4rem" }}>
      <h1>500</h1>
      <p>Đã xảy ra lỗi máy chủ. Vui lòng thử lại sau.</p>
      <Link to="/">Về trang chủ</Link>
    </div>
  );
}
