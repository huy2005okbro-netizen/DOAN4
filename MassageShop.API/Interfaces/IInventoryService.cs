using MassageShop.API.Models.Entities;

namespace MassageShop.API.Interfaces
{
    public interface IInventoryService
    {
        Task<List<InventoryTransaction>> GetAllAsync();
        Task<List<InventoryTransaction>> GetByProductIdAsync(int productId);
        Task<InventoryTransaction> ImportAsync(int productId, int quantity, string? note);
        Task<InventoryTransaction> AdjustAsync(int productId, int quantity, string? note);
    }
}
