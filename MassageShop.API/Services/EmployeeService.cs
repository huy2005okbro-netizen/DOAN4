using MassageShop.API.Data;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.DTOs.Employee;
using MassageShop.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _db;

        public EmployeeService(AppDbContext db) => _db = db;

        public async Task<List<EmployeeResponseDto>> GetAllAsync()
        {
            return await _db.Employees
                .Include(e => e.User)
                .Select(e => MapToDto(e))
                .ToListAsync();
        }

        public async Task<EmployeeResponseDto?> GetByIdAsync(int id)
        {
            var e = await _db.Employees.Include(e => e.User).FirstOrDefaultAsync(e => e.Id == id);
            return e == null ? null : MapToDto(e);
        }

        public async Task<EmployeeResponseDto> CreateAsync(EmployeeCreateDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email.ToLower()))
                throw new InvalidOperationException("Email đã tồn tại");
            if (await _db.Users.AnyAsync(u => u.Phone == dto.Phone))
                throw new InvalidOperationException("Số điện thoại đã tồn tại");

            var role = await _db.Roles.FirstAsync(r => r.Name == "EMPLOYEE");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email.ToLower(),
                Phone = dto.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = role.Id
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var employee = new Employee
            {
                UserId = user.Id,
                Position = dto.Position,
                StartDate = dto.StartDate
            };
            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();

            employee.User = user;
            return MapToDto(employee);
        }

        public async Task<EmployeeResponseDto?> UpdateAsync(int id, EmployeeUpdateDto dto)
        {
            var emp = await _db.Employees.Include(e => e.User).FirstOrDefaultAsync(e => e.Id == id);
            if (emp == null) return null;

            if (dto.FullName != null) emp.User.FullName = dto.FullName;
            if (dto.Phone != null)
            {
                if (await _db.Users.AnyAsync(u => u.Phone == dto.Phone && u.Id != emp.UserId))
                    throw new InvalidOperationException("Số điện thoại đã tồn tại");
                emp.User.Phone = dto.Phone;
            }
            if (dto.Position != null) emp.Position = dto.Position;
            if (dto.StartDate.HasValue) emp.StartDate = dto.StartDate.Value;
            if (dto.IsActive.HasValue) emp.IsActive = dto.IsActive.Value;

            await _db.SaveChangesAsync();
            return MapToDto(emp);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var emp = await _db.Employees.Include(e => e.User).FirstOrDefaultAsync(e => e.Id == id);
            if (emp == null) return false;

            emp.IsActive = false;
            emp.User.IsActive = false;
            await _db.SaveChangesAsync();
            return true;
        }

        private static EmployeeResponseDto MapToDto(Employee e) => new()
        {
            Id = e.Id,
            UserId = e.UserId,
            FullName = e.User.FullName,
            Email = e.User.Email,
            Phone = e.User.Phone,
            Position = e.Position,
            StartDate = e.StartDate,
            IsActive = e.IsActive
        };
    }
}
