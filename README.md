# Đồ Án 4 - Web Application

Ứng dụng web full-stack sử dụng **React + Node.js/Express + MongoDB**.

---

## Cấu trúc dự án

```
Đồ An 4/
├── backend/                  # Node.js + Express API
│   ├── src/
│   │   ├── config/           # Cấu hình database
│   │   ├── controllers/      # Xử lý logic request
│   │   ├── middleware/       # Auth & error middleware
│   │   ├── models/           # Mongoose models
│   │   ├── routes/           # API routes
│   │   └── server.js         # Entry point
│   ├── .env                  # Biến môi trường
│   └── package.json
│
├── frontend/                 # React App
│   ├── public/
│   ├── src/
│   │   ├── api/              # Axios API calls
│   │   ├── components/       # Components tái sử dụng
│   │   │   ├── common/       # PrivateRoute, ...
│   │   │   └── layout/       # Navbar, Footer, ...
│   │   ├── context/          # React Context (Auth)
│   │   ├── pages/            # Các trang: Home, Login, ...
│   │   ├── App.js
│   │   └── index.js
│   ├── .env
│   └── package.json
│
└── README.md
```

---

## Cài đặt & Chạy

### Yêu cầu

- Node.js >= 16
- MongoDB (local hoặc MongoDB Atlas)

### Backend

```bash
cd backend
npm install
# Chỉnh sửa .env nếu cần
npm run dev
```

### Frontend

```bash
cd frontend
npm install
npm start
```

---

## API Endpoints

| Method | Endpoint           | Mô tả                  | Auth  |
| ------ | ------------------ | ---------------------- | ----- |
| POST   | /api/auth/register | Đăng ký tài khoản      | ❌    |
| POST   | /api/auth/login    | Đăng nhập              | ❌    |
| GET    | /api/auth/me       | Lấy thông tin bản thân | ✅    |
| GET    | /api/users         | Lấy danh sách user     | Admin |
| GET    | /api/users/:id     | Lấy user theo ID       | ✅    |
| PUT    | /api/users/:id     | Cập nhật user          | ✅    |
| DELETE | /api/users/:id     | Xóa user               | Admin |

---

## Công nghệ sử dụng

**Backend:** Node.js, Express, MongoDB, Mongoose, JWT, bcryptjs  
**Frontend:** React 18, React Router v6, Axios, React Toastify
