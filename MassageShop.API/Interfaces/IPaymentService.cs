using MassageShop.API.Models.DTOs.Payment;
using MassageShop.API.Models.Entities;

namespace MassageShop.API.Interfaces
{
    public interface IPaymentService
    {
        Task<List<PaymentResponseDto>> GetAllAsync();
        Task<PaymentResponseDto?> GetByIdAsync(int id);
        Task<PaymentResponseDto?> GetByOrderIdAsync(int orderId);
        Task<PaymentResponseDto> CreateAsync(CreatePaymentDto dto);
        Task<PaymentResponseDto?> UpdateStatusAsync(int id, PaymentStatus status);
    }
}
