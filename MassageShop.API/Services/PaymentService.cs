using MassageShop.API.Data;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.DTOs.Payment;
using MassageShop.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _db;

        public PaymentService(AppDbContext db) => _db = db;

        public async Task<List<PaymentResponseDto>> GetAllAsync() =>
            await QueryWithIncludes().Select(p => MapToDto(p)).ToListAsync();

        public async Task<PaymentResponseDto?> GetByIdAsync(int id)
        {
            var p = await QueryWithIncludes().FirstOrDefaultAsync(p => p.Id == id);
            return p == null ? null : MapToDto(p);
        }

        public async Task<PaymentResponseDto?> GetByOrderIdAsync(int orderId)
        {
            var p = await QueryWithIncludes().FirstOrDefaultAsync(p => p.OrderId == orderId);
            return p == null ? null : MapToDto(p);
        }

        public async Task<PaymentResponseDto> CreateAsync(CreatePaymentDto dto)
        {
            var order = await _db.Orders.FindAsync(dto.OrderId)
                ?? throw new KeyNotFoundException("Đơn hàng không tồn tại");

            if (await _db.Payments.AnyAsync(p => p.OrderId == dto.OrderId))
                throw new InvalidOperationException("Đơn hàng này đã có thông tin thanh toán");

            var payment = new Payment
            {
                OrderId = dto.OrderId,
                PaymentMethod = dto.PaymentMethod,
                Amount = order.TotalAmount,
                TransactionCode = dto.TransactionCode
            };
            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();

            return (await GetByIdAsync(payment.Id))!;
        }

        public async Task<PaymentResponseDto?> UpdateStatusAsync(int id, PaymentStatus status)
        {
            var payment = await _db.Payments.Include(p => p.Order).FirstOrDefaultAsync(p => p.Id == id);
            if (payment == null) return null;

            payment.Status = status;
            if (status == PaymentStatus.PAID)
            {
                payment.PaidAt = DateTime.UtcNow;
                // Cập nhật trạng thái đơn hàng sang CONFIRMED nếu chưa
                if (payment.Order.Status == OrderStatus.PENDING)
                    payment.Order.Status = OrderStatus.CONFIRMED;
            }

            await _db.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        private IQueryable<Payment> QueryWithIncludes() =>
            _db.Payments.Include(p => p.Order);

        private static PaymentResponseDto MapToDto(Payment p) => new()
        {
            Id = p.Id,
            OrderId = p.OrderId,
            OrderCode = p.Order.OrderCode,
            PaymentMethod = p.PaymentMethod.ToString(),
            Amount = p.Amount,
            Status = p.Status,
            TransactionCode = p.TransactionCode,
            PaidAt = p.PaidAt
        };
    }
}
