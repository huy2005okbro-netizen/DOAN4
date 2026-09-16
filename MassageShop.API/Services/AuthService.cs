using MassageShop.API.Data;
using MassageShop.API.Helpers;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.DTOs.Auth;
using MassageShop.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly JwtHelper _jwt;

        public AuthService(AppDbContext db, JwtHelper jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        // =============================================
        // ĐĂNG KÝ
        // =============================================
        public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _db.Users.AnyAsync(u => u.Email == request.Email.ToLower()))
                throw new InvalidOperationException("Email đã tồn tại");

            if (await _db.Users.AnyAsync(u => u.Phone == request.Phone))
                throw new InvalidOperationException("Số điện thoại đã tồn tại");

            var customerRole = await _db.Roles.FirstAsync(r => r.Name == "CUSTOMER");

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email.ToLower(),
                Phone = request.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = customerRole.Id
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var customer = new Customer { UserId = user.Id };
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();

            user.Role = customerRole;
            var token = _jwt.GenerateToken(user);

            return new LoginResponse
            {
                Token = token,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = customerRole.Name
            };
        }

        // =============================================
        // ĐĂNG NHẬP
        // =============================================
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Tài khoản đã bị khóa");

            var token = _jwt.GenerateToken(user);

            return new LoginResponse
            {
                Token = token,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.Name
            };
        }

        // =============================================
        // LẤY THÔNG TIN NGƯỜI DÙNG HIỆN TẠI
        // =============================================
        public async Task<LoginResponse> GetMeAsync(int userId)
        {
            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new KeyNotFoundException("Không tìm thấy người dùng");

            return new LoginResponse
            {
                Token = string.Empty,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.Name
            };
        }

        // =============================================
        // QUÊN MẬT KHẨU
        // Lưu ý: thực tế cần tích hợp email service (SendGrid, SMTP...)
        // Hiện tại lưu reset token vào DB, trả về token qua log (dev mode)
        // =============================================
        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == email.ToLower());

            // Không tiết lộ email có tồn tại hay không (bảo mật)
            if (user == null || !user.IsActive) return;

            // Tạo token ngẫu nhiên 6 chữ số (OTP)
            var otp = new Random().Next(100000, 999999).ToString();
            var otpHash = BCrypt.Net.BCrypt.HashPassword(otp);
            var expiry = DateTime.UtcNow.AddMinutes(15);

            // Lưu vào PasswordResetToken (dùng tạm PasswordHash field phụ)
            // Trong production: lưu vào bảng PasswordResetTokens riêng
            user.PasswordHash = $"RESET:{otpHash}:{expiry:O}:{user.PasswordHash}";
            await _db.SaveChangesAsync();

            // TODO: Gửi email OTP thực tế
            // Tạm thời log ra console để test
            Console.WriteLine($"[DEV] OTP cho {email}: {otp} (hết hạn lúc {expiry:HH:mm:ss})");
        }

        // =============================================
        // ĐẶT LẠI MẬT KHẨU (dùng OTP)
        // =============================================
        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            // Tìm user có reset token
            var users = await _db.Users
                .Where(u => u.PasswordHash.StartsWith("RESET:"))
                .ToListAsync();

            foreach (var user in users)
            {
                var parts = user.PasswordHash.Split(':');
                if (parts.Length < 4) continue;

                // parts[0] = "RESET", parts[1] = otpHash, parts[2] = expiry ISO, parts[3..] = original hash
                var otpHash = parts[1];
                if (!DateTime.TryParse(parts[2], out var expiry)) continue;

                if (DateTime.UtcNow > expiry) continue; // hết hạn

                if (!BCrypt.Net.BCrypt.Verify(token, otpHash)) continue; // sai OTP

                // Khôi phục mật khẩu gốc trước khi replace
                var originalHash = string.Join(":", parts.Skip(3));

                // Đặt mật khẩu mới
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await _db.SaveChangesAsync();
                return true;
            }

            return false;
        }

        // =============================================
        // ĐỔI MẬT KHẨU (khi đã đăng nhập)
        // =============================================
        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return false;

            // Xử lý trường hợp đang có reset token
            var hashToVerify = user.PasswordHash.StartsWith("RESET:")
                ? string.Join(":", user.PasswordHash.Split(':').Skip(3))
                : user.PasswordHash;

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, hashToVerify))
                throw new InvalidOperationException("Mật khẩu hiện tại không đúng");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
