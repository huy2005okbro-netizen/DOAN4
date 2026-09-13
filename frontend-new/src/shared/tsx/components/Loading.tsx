export default function Loading() {
  return (
    <div style={{ display: "flex", justifyContent: "center", padding: "2rem" }}>
      <div className="spinner" aria-label="Đang tải..." />
    </div>
  );
}
