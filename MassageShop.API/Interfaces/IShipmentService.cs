using MassageShop.API.Models.DTOs.Shipment;
using MassageShop.API.Models.Entities;

namespace MassageShop.API.Interfaces
{
    public interface IShipmentService
    {
        Task<List<ShipmentResponseDto>> GetAllAsync();
        Task<ShipmentResponseDto?> GetByIdAsync(int id);
        Task<ShipmentResponseDto?> GetByOrderIdAsync(int orderId);
        Task<ShipmentResponseDto> CreateAsync(CreateShipmentDto dto);
        Task<ShipmentResponseDto?> UpdateStatusAsync(int id, ShipmentStatus status);
    }
}
