using MassageShop.API.Models.Entities;

namespace MassageShop.API.Interfaces
{
    public interface IServiceCategoryService
    {
        Task<List<ServiceCategory>> GetAllAsync();
        Task<ServiceCategory?> GetByIdAsync(int id);
        Task<ServiceCategory> CreateAsync(string name, string? description);
        Task<ServiceCategory?> UpdateAsync(int id, string? name, string? description, bool? isActive);
        Task<bool> DeleteAsync(int id);
    }
}
