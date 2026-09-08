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

        public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
        {
            // Kiểm tra email trùng
            if (await _db.Users.AnyAsync(u => u.Email == request.Email.ToLower()))
                throw new InvalidOperationException("Email đã tồn tại");

            // Kiểm tra phone trùng
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

            // Tạo Customer tương ứng
            var customer = new Customer { UserId = user.Id };
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();

            // Load role để tạo token
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
    }
}
