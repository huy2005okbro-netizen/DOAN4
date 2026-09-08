using MassageShop.API.Data;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Services
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly AppDbContext _db;

        public ProductCategoryService(AppDbContext db) => _db = db;

        public async Task<List<ProductCategory>> GetAllAsync() =>
            await _db.ProductCategories.ToListAsync();

        public async Task<ProductCategory?> GetByIdAsync(int id) =>
            await _db.ProductCategories.FindAsync(id);

        public async Task<ProductCategory> CreateAsync(string name, string? description)
        {
            var cat = new ProductCategory { Name = name, Description = description };
            _db.ProductCategories.Add(cat);
            await _db.SaveChangesAsync();
            return cat;
        }

        public async Task<ProductCategory?> UpdateAsync(int id, string? name, string? description, bool? isActive)
        {
            var cat = await _db.ProductCategories.FindAsync(id);
            if (cat == null) return null;

            if (name != null) cat.Name = name;
            if (description != null) cat.Description = description;
            if (isActive.HasValue) cat.IsActive = isActive.Value;

            await _db.SaveChangesAsync();
            return cat;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cat = await _db.ProductCategories.FindAsync(id);
            if (cat == null) return false;
            _db.ProductCategories.Remove(cat);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
