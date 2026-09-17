using MassageShop.API.Data;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.DTOs.Customer;
using MassageShop.API.Models.DTOs.Employee;
using MassageShop.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Controllers
{
    /// <summary>
    /// Quản lý tài khoản thống nhất — ADMIN only
    /// Tổng hợp Admin + Employee + Customer thành một danh sách
    /// </summary>
    [ApiController]
    [Route("api/accounts")]
    [Authorize(Roles = "ADMIN")]
    public class AccountsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IEmployeeService _employeeService;
        private readonly ICustomerService _customerService;

        public AccountsController(
            AppDbContext db,
            IEmployeeService employeeService,
            ICustomerService customerService)
        {
            _db = db;
            _employeeService = employeeService;
            _customerService = customerService;
        }

        // ============================================================
        // GET ALL — unified list
        // ============================================================
        /// <summary>
        /// Lấy toàn bộ tài khoản (Admin + Employee + Customer).
        /// Hỗ trợ filter: role, isActive, search (name/email/phone).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? role,
            [FromQuery] bool? isActive,
            [FromQuery] string? search)
        {
            var query = _db.Users
                .Include(u => u.Role)
                .Include(u => u.Employee)
                .Include(u => u.Customer)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(role))
                query = query.Where(u => u.Role.Name == role.ToUpper());

            if (isActive.HasValue)
                query = query.Where(u => u.IsActive == isActive.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(s) ||
                    u.Email.ToLower().Contains(s) ||
                    u.Phone.Contains(s));
            }

            var users = await query
                .OrderBy(u => u.RoleId)
                .ThenBy(u => u.CreatedAt)
                .ToListAsync();

            var result = users.Select(u => MapToAccountDto(u)).ToList();
            return Ok(result);
        }

        // ============================================================
        // GET STATS
        // ============================================================
        /// <summary>Thống kê số lượng theo vai trò</summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var total = await _db.Users.CountAsync();
            var admins = await _db.Users.CountAsync(u => u.Role.Name == "ADMIN");
            var employees = await _db.Users.CountAsync(u => u.Role.Name == "EMPLOYEE");
            var customers = await _db.Users.CountAsync(u => u.Role.Name == "CUSTOMER");
            var active = await _db.Users.CountAsync(u => u.IsActive);
            var locked = await _db.Users.CountAsync(u => !u.IsActive);

            return Ok(new { Total = total, Admins = admins, Employees = employees, Customers = customers, Active = active, Locked = locked });
        }

        // ============================================================
        // GET BY ID
        // ============================================================
        /// <summary>Lấy chi tiết tài khoản theo userId</summary>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetById(int userId)
        {
            var user = await _db.Users
                .Include(u => u.Role)
                .Include(u => u.Employee)
                .Include(u => u.Customer)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound(new { message = "Không tìm thấy tài khoản" });

            return Ok(MapToAccountDto(user));
        }

        // ============================================================
        // CREATE — tạo employee hoặc customer
        // ============================================================
        /// <summary>Tạo tài khoản nhân viên mới</summary>
        [HttpPost("employee")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeCreateDto dto)
        {
            try
            {
                var result = await _employeeService.CreateAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Tạo tài khoản khách hàng mới (admin tạo thay)</summary>
        [HttpPost("customer")]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerCreateDto dto)
        {
            try
            {
                var result = await _customerService.CreateAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ============================================================
        // UPDATE EMPLOYEE
        // ============================================================
        /// <summary>Cập nhật thông tin nhân viên theo employeeId</summary>
        [HttpPut("employee/{employeeId}")]
        public async Task<IActionResult> UpdateEmployee(int employeeId, [FromBody] EmployeeUpdateDto dto)
        {
            try
            {
                var result = await _employeeService.UpdateAsync(employeeId, dto);
                if (result == null) return NotFound(new { message = "Không tìm thấy nhân viên" });
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Cập nhật thông tin khách hàng theo customerId</summary>
        [HttpPut("customer/{customerId}")]
        public async Task<IActionResult> UpdateCustomer(int customerId, [FromBody] CustomerUpdateDto dto)
        {
            try
            {
                var result = await _customerService.UpdateAsync(customerId, dto);
                if (result == null) return NotFound(new { message = "Không tìm thấy khách hàng" });
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Cập nhật thông tin cơ bản của tài khoản quản trị viên.</summary>
        [HttpPut("admin/{userId}")]
        public async Task<IActionResult> UpdateAdmin(int userId, [FromBody] AdminAccountUpdateDto dto)
        {
            var user = await _db.Users
                .Include(u => u.Role)
                .Include(u => u.Employee)
                .Include(u => u.Customer)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.Role.Name != "ADMIN")
                return NotFound(new { message = "Không tìm thấy tài khoản quản trị viên" });

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.FullName = dto.FullName.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Phone))
            {
                var phoneInUse = await _db.Users
                    .AnyAsync(u => u.Phone == dto.Phone && u.Id != user.Id);
                if (phoneInUse)
                    return BadRequest(new { message = "Số điện thoại đã tồn tại" });

                user.Phone = dto.Phone.Trim();
            }

            await _db.SaveChangesAsync();
            return Ok(MapToAccountDto(user));
        }

        // ============================================================
        // LOCK / UNLOCK — dùng IsActive
        // ============================================================
        /// <summary>Khóa hoặc mở khóa tài khoản theo userId</summary>
        [HttpPatch("{userId}/toggle-active")]
        public async Task<IActionResult> ToggleActive(int userId, [FromBody] ToggleActiveRequest req)
        {
            var user = await _db.Users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound(new { message = "Không tìm thấy tài khoản" });

            // Không cho khóa chính mình
            var callerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (user.Id == callerId)
                return BadRequest(new { message = "Không thể khóa tài khoản của chính mình" });

            user.IsActive = req.IsActive;
            if (user.Employee != null)
                user.Employee.IsActive = req.IsActive;

            await _db.SaveChangesAsync();

            var action = req.IsActive ? "mở khóa" : "khóa";
            return Ok(new { message = $"Đã {action} tài khoản thành công", isActive = req.IsActive });
        }

        // ============================================================
        // RESET PASSWORD (admin force reset)
        // ============================================================
        /// <summary>Admin reset mật khẩu cho tài khoản bất kỳ</summary>
        [HttpPost("{userId}/reset-password")]
        public async Task<IActionResult> AdminResetPassword(int userId, [FromBody] AdminResetPasswordRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.NewPassword) || req.NewPassword.Length < 6)
                return BadRequest(new { message = "Mật khẩu mới tối thiểu 6 ký tự" });

            var user = await _db.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new { message = "Không tìm thấy tài khoản" });

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Đã reset mật khẩu thành công" });
        }

        // ============================================================
        // MAPPER
        // ============================================================
        private static AccountDto MapToAccountDto(User u) => new()
        {
            UserId = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            Phone = u.Phone,
            Role = u.Role.Name,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            // Employee-specific
            EmployeeId = u.Employee?.Id,
            Position = u.Employee?.Position,
            StartDate = u.Employee?.StartDate,
            // Customer-specific
            CustomerId = u.Customer?.Id,
            LoyaltyPoints = u.Customer?.LoyaltyPoints,
            Address = u.Customer?.Address,
            DateOfBirth = u.Customer?.DateOfBirth,
            Gender = u.Customer?.Gender,
        };
    }

    // ============================================================
    // Request / Response DTOs
    // ============================================================
    public class AccountDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        // Employee fields
        public int? EmployeeId { get; set; }
        public string? Position { get; set; }
        public DateTime? StartDate { get; set; }
        // Customer fields
        public int? CustomerId { get; set; }
        public int? LoyaltyPoints { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
    }

    public class ToggleActiveRequest
    {
        public bool IsActive { get; set; }
    }

    public class AdminResetPasswordRequest
    {
        public string NewPassword { get; set; } = string.Empty;
    }

    public class AdminAccountUpdateDto
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
    }
}
