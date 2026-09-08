using MassageShop.API.Data;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.DTOs.Customer;
using MassageShop.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _db;

        public CustomerService(AppDbContext db) => _db = db;

        public async Task<List<CustomerResponseDto>> GetAllAsync()
        {
            return await _db.Customers
                .Include(c => c.User)
                .Select(c => MapToDto(c))
                .ToListAsync();
        }

        public async Task<CustomerResponseDto?> GetByIdAsync(int id)
        {
            var c = await _db.Customers.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id);
            return c == null ? null : MapToDto(c);
        }

        public async Task<CustomerResponseDto?> GetByUserIdAsync(int userId)
        {
            var c = await _db.Customers.Include(c => c.User).FirstOrDefaultAsync(c => c.UserId == userId);
            return c == null ? null : MapToDto(c);
        }

        public async Task<CustomerResponseDto> CreateAsync(CustomerCreateDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email.ToLower()))
                throw new InvalidOperationException("Email đã tồn tại");
            if (await _db.Users.AnyAsync(u => u.Phone == dto.Phone))
                throw new InvalidOperationException("Số điện thoại đã tồn tại");

            var role = await _db.Roles.FirstAsync(r => r.Name == "CUSTOMER");

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

            var customer = new Customer
            {
                UserId = user.Id,
                Address = dto.Address,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender
            };
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();

            customer.User = user;
            return MapToDto(customer);
        }

        public async Task<CustomerResponseDto?> UpdateAsync(int id, CustomerUpdateDto dto)
        {
            var customer = await _db.Customers.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id);
            if (customer == null) return null;

            if (dto.FullName != null) customer.User.FullName = dto.FullName;
            if (dto.Phone != null)
            {
                if (await _db.Users.AnyAsync(u => u.Phone == dto.Phone && u.Id != customer.UserId))
                    throw new InvalidOperationException("Số điện thoại đã tồn tại");
                customer.User.Phone = dto.Phone;
            }
            if (dto.Address != null) customer.Address = dto.Address;
            if (dto.DateOfBirth.HasValue) customer.DateOfBirth = dto.DateOfBirth;
            if (dto.Gender != null) customer.Gender = dto.Gender;

            await _db.SaveChangesAsync();
            return MapToDto(customer);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _db.Customers.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id);
            if (customer == null) return false;

            customer.User.IsActive = false;
            await _db.SaveChangesAsync();
            return true;
        }

        private static CustomerResponseDto MapToDto(Customer c) => new()
        {
            Id = c.Id,
            UserId = c.UserId,
            FullName = c.User.FullName,
            Email = c.User.Email,
            Phone = c.User.Phone,
            Address = c.Address,
            DateOfBirth = c.DateOfBirth,
            Gender = c.Gender,
            LoyaltyPoints = c.LoyaltyPoints,
            IsActive = c.User.IsActive,
            CreatedAt = c.CreatedAt
        };
    }
}
