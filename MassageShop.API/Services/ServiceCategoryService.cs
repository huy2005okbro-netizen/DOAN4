using MassageShop.API.Data;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Services
{
    public class ServiceCategoryService : IServiceCategoryService
    {
        private readonly AppDbContext _db;

        public ServiceCategoryService(AppDbContext db) => _db = db;

        public async Task<List<ServiceCategory>> GetAllAsync() =>
            await _db.ServiceCategories.ToListAsync();

        public async Task<ServiceCategory?> GetByIdAsync(int id) =>
            await _db.ServiceCategories.FindAsync(id);

        public async Task<ServiceCategory> CreateAsync(string name, string? description)
        {
            var cat = new ServiceCategory { Name = name, Description = description };
            _db.ServiceCategories.Add(cat);
            await _db.SaveChangesAsync();
            return cat;
        }

        public async Task<ServiceCategory?> UpdateAsync(int id, string? name, string? description, bool? isActive)
        {
            var cat = await _db.ServiceCategories.FindAsync(id);
            if (cat == null) return null;

            if (name != null) cat.Name = name;
            if (description != null) cat.Description = description;
            if (isActive.HasValue) cat.IsActive = isActive.Value;

            await _db.SaveChangesAsync();
            return cat;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cat = await _db.ServiceCategories.FindAsync(id);
            if (cat == null) return false;
            _db.ServiceCategories.Remove(cat);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
