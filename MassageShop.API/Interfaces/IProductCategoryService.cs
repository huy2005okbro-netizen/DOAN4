using MassageShop.API.Models.Entities;

namespace MassageShop.API.Interfaces
{
    public interface IProductCategoryService
    {
        Task<List<ProductCategory>> GetAllAsync();
        Task<ProductCategory?> GetByIdAsync(int id);
        Task<ProductCategory> CreateAsync(string name, string? description);
        Task<ProductCategory?> UpdateAsync(int id, string? name, string? description, bool? isActive);
        Task<bool> DeleteAsync(int id);
    }
}
