using MassageShop.API.Data;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.DTOs.Product;
using MassageShop.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;

        public ProductService(AppDbContext db) => _db = db;

        public async Task<List<ProductResponseDto>> GetAllAsync() =>
            await _db.Products
                .Include(p => p.ProductCategory)
                .Select(p => MapToDto(p))
                .ToListAsync();

        public async Task<ProductResponseDto?> GetByIdAsync(int id)
        {
            var p = await _db.Products.Include(p => p.ProductCategory).FirstOrDefaultAsync(p => p.Id == id);
            return p == null ? null : MapToDto(p);
        }

        public async Task<ProductResponseDto> CreateAsync(ProductCreateDto dto)
        {
            var cat = await _db.ProductCategories.FindAsync(dto.ProductCategoryId)
                ?? throw new KeyNotFoundException("Danh mục sản phẩm không tồn tại");

            var product = new Product
            {
                ProductCategoryId = dto.ProductCategoryId,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CostPrice = dto.CostPrice,
                StockQuantity = dto.StockQuantity,
                ImageUrl = dto.ImageUrl,
                Brand = dto.Brand
            };
            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            product.ProductCategory = cat;
            return MapToDto(product);
        }

        public async Task<ProductResponseDto?> UpdateAsync(int id, ProductUpdateDto dto)
        {
            var product = await _db.Products.Include(p => p.ProductCategory).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return null;

            if (dto.ProductCategoryId.HasValue)
            {
                var cat = await _db.ProductCategories.FindAsync(dto.ProductCategoryId.Value)
                    ?? throw new KeyNotFoundException("Danh mục không tồn tại");
                product.ProductCategoryId = dto.ProductCategoryId.Value;
                product.ProductCategory = cat;
            }
            if (dto.Name != null) product.Name = dto.Name;
            if (dto.Description != null) product.Description = dto.Description;
            if (dto.Price.HasValue) product.Price = dto.Price.Value;
            if (dto.CostPrice.HasValue) product.CostPrice = dto.CostPrice.Value;
            if (dto.ImageUrl != null) product.ImageUrl = dto.ImageUrl;
            if (dto.Brand != null) product.Brand = dto.Brand;
            if (dto.IsActive.HasValue) product.IsActive = dto.IsActive.Value;

            await _db.SaveChangesAsync();
            return MapToDto(product);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return false;
            product.IsActive = false;
            await _db.SaveChangesAsync();
            return true;
        }

        private static ProductResponseDto MapToDto(Product p) => new()
        {
            Id = p.Id,
            ProductCategoryId = p.ProductCategoryId,
            CategoryName = p.ProductCategory.Name,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            CostPrice = p.CostPrice,
            StockQuantity = p.StockQuantity,
            ImageUrl = p.ImageUrl,
            Brand = p.Brand,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt
        };
    }
}
