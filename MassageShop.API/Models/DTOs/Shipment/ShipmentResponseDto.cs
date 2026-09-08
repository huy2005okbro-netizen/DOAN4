using MassageShop.API.Models.Entities;

namespace MassageShop.API.Models.DTOs.Shipment
{
    public class ShipmentResponseDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public int? ShippingMethodId { get; set; }
        public string? ShippingMethodName { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string RecipientPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public decimal ShippingFee { get; set; }
        public string? TrackingCode { get; set; }
        public ShipmentStatus Status { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }
}
