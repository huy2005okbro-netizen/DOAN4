using MassageShop.API.Data;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.DTOs.Service;
using MassageShop.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Services
{
    public class ServicesService : IServicesService
    {
        private readonly AppDbContext _db;

        public ServicesService(AppDbContext db) => _db = db;

        public async Task<List<ServiceResponseDto>> GetAllAsync() =>
            await _db.Services
                .Include(s => s.ServiceCategory)
                .Select(s => MapToDto(s))
                .ToListAsync();

        public async Task<ServiceResponseDto?> GetByIdAsync(int id)
        {
            var s = await _db.Services.Include(s => s.ServiceCategory).FirstOrDefaultAsync(s => s.Id == id);
            return s == null ? null : MapToDto(s);
        }

        public async Task<ServiceResponseDto> CreateAsync(ServiceCreateDto dto)
        {
            var cat = await _db.ServiceCategories.FindAsync(dto.ServiceCategoryId)
                ?? throw new KeyNotFoundException("Danh mục dịch vụ không tồn tại");

            var service = new Service
            {
                ServiceCategoryId = dto.ServiceCategoryId,
                Name = dto.Name,
                Description = dto.Description,
                Duration = dto.Duration,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl
            };
            _db.Services.Add(service);
            await _db.SaveChangesAsync();

            service.ServiceCategory = cat;
            return MapToDto(service);
        }

        public async Task<ServiceResponseDto?> UpdateAsync(int id, ServiceUpdateDto dto)
        {
            var service = await _db.Services.Include(s => s.ServiceCategory).FirstOrDefaultAsync(s => s.Id == id);
            if (service == null) return null;

            if (dto.ServiceCategoryId.HasValue)
            {
                var cat = await _db.ServiceCategories.FindAsync(dto.ServiceCategoryId.Value)
                    ?? throw new KeyNotFoundException("Danh mục dịch vụ không tồn tại");
                service.ServiceCategoryId = dto.ServiceCategoryId.Value;
                service.ServiceCategory = cat;
            }
            if (dto.Name != null) service.Name = dto.Name;
            if (dto.Description != null) service.Description = dto.Description;
            if (dto.Duration.HasValue) service.Duration = dto.Duration.Value;
            if (dto.Price.HasValue) service.Price = dto.Price.Value;
            if (dto.ImageUrl != null) service.ImageUrl = dto.ImageUrl;
            if (dto.IsActive.HasValue) service.IsActive = dto.IsActive.Value;

            await _db.SaveChangesAsync();
            return MapToDto(service);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var service = await _db.Services.FindAsync(id);
            if (service == null) return false;
            service.IsActive = false;
            await _db.SaveChangesAsync();
            return true;
        }

        private static ServiceResponseDto MapToDto(Service s) => new()
        {
            Id = s.Id,
            ServiceCategoryId = s.ServiceCategoryId,
            CategoryName = s.ServiceCategory.Name,
            Name = s.Name,
            Description = s.Description,
            Duration = s.Duration,
            Price = s.Price,
            ImageUrl = s.ImageUrl,
            IsActive = s.IsActive
        };
    }
}
