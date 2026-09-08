using MassageShop.API.Models.Entities;

namespace MassageShop.API.Interfaces
{
    public interface IInvoiceService
    {
        Task<List<Invoice>> GetAllAsync();
        Task<Invoice?> GetByIdAsync(int id);
        Task<Invoice?> GetByOrderIdAsync(int orderId);
        Task<Invoice> GenerateAsync(int orderId);
    }
}
