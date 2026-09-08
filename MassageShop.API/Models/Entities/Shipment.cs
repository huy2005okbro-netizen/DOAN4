using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MassageShop.API.Models.Entities
{
    public enum ShipmentStatus
    {
        PENDING,
        PREPARING,
        SHIPPED,
        DELIVERING,
        DELIVERED,
        FAILED,
        RETURNED
    }

    public class Shipment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; } = null!;

        public int? ShippingMethodId { get; set; }

        [ForeignKey(nameof(ShippingMethodId))]
        public ShippingMethod? ShippingMethod { get; set; }

        [Required, MaxLength(100)]
        public string RecipientName { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string RecipientPhone { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string ShippingAddress { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; }

        [MaxLength(100)]
        public string? TrackingCode { get; set; }

        public ShipmentStatus Status { get; set; } = ShipmentStatus.PENDING;

        public DateTime? ShippedAt { get; set; }

        public DateTime? DeliveredAt { get; set; }
    }
}
