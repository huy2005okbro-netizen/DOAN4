using MassageShop.API.Data;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.DTOs.Shipment;
using MassageShop.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly AppDbContext _db;

        public ShipmentService(AppDbContext db) => _db = db;

        public async Task<List<ShipmentResponseDto>> GetAllAsync() =>
            await QueryWithIncludes().Select(s => MapToDto(s)).ToListAsync();

        public async Task<ShipmentResponseDto?> GetByIdAsync(int id)
        {
            var s = await QueryWithIncludes().FirstOrDefaultAsync(s => s.Id == id);
            return s == null ? null : MapToDto(s);
        }

        public async Task<ShipmentResponseDto?> GetByOrderIdAsync(int orderId)
        {
            var s = await QueryWithIncludes().FirstOrDefaultAsync(s => s.OrderId == orderId);
            return s == null ? null : MapToDto(s);
        }

        public async Task<ShipmentResponseDto> CreateAsync(CreateShipmentDto dto)
        {
            var order = await _db.Orders.FindAsync(dto.OrderId)
                ?? throw new KeyNotFoundException("Đơn hàng không tồn tại");

            if (await _db.Shipments.AnyAsync(s => s.OrderId == dto.OrderId))
                throw new InvalidOperationException("Đơn hàng này đã có vận đơn");

            decimal fee = 0;
            if (dto.ShippingMethodId.HasValue)
            {
                var method = await _db.ShippingMethods.FindAsync(dto.ShippingMethodId.Value);
                fee = method?.Fee ?? 0;
            }

            var shipment = new Shipment
            {
                OrderId = dto.OrderId,
                ShippingMethodId = dto.ShippingMethodId,
                RecipientName = dto.RecipientName,
                RecipientPhone = dto.RecipientPhone,
                ShippingAddress = dto.ShippingAddress,
                ShippingFee = fee
            };
            _db.Shipments.Add(shipment);
            await _db.SaveChangesAsync();

            return (await GetByIdAsync(shipment.Id))!;
        }

        public async Task<ShipmentResponseDto?> UpdateStatusAsync(int id, ShipmentStatus status)
        {
            var shipment = await _db.Shipments.FindAsync(id);
            if (shipment == null) return null;

            shipment.Status = status;
            if (status == ShipmentStatus.SHIPPED) shipment.ShippedAt = DateTime.UtcNow;
            if (status == ShipmentStatus.DELIVERED) shipment.DeliveredAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        private IQueryable<Shipment> QueryWithIncludes() =>
            _db.Shipments
                .Include(s => s.Order)
                .Include(s => s.ShippingMethod);

        private static ShipmentResponseDto MapToDto(Shipment s) => new()
        {
            Id = s.Id,
            OrderId = s.OrderId,
            OrderCode = s.Order.OrderCode,
            ShippingMethodId = s.ShippingMethodId,
            ShippingMethodName = s.ShippingMethod?.Name,
            RecipientName = s.RecipientName,
            RecipientPhone = s.RecipientPhone,
            ShippingAddress = s.ShippingAddress,
            ShippingFee = s.ShippingFee,
            TrackingCode = s.TrackingCode,
            Status = s.Status,
            ShippedAt = s.ShippedAt,
            DeliveredAt = s.DeliveredAt
        };
    }
}
