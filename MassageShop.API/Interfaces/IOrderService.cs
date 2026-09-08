using MassageShop.API.Models.DTOs.Order;
using MassageShop.API.Models.Entities;

namespace MassageShop.API.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderResponseDto>> GetAllAsync();
        Task<List<OrderResponseDto>> GetByCustomerIdAsync(int customerId);
        Task<OrderResponseDto?> GetByIdAsync(int id);
        Task<OrderResponseDto> CreateAsync(int customerId, CreateOrderDto dto);
        Task<OrderResponseDto?> UpdateStatusAsync(int id, OrderStatus status);
        Task<bool> CancelAsync(int id, int requesterId, string requesterRole);
    }
}
