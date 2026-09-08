using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MassageShop.API.Models.Entities
{
    public enum OrderType
    {
        IN_STORE,
        ONLINE
    }

    public enum OrderStatus
    {
        PENDING,
        CONFIRMED,
        PREPARING,
        HANDED_TO_SHIPPING,
        DELIVERING,
        DELIVERED,
        COMPLETED,
        CANCELLED
    }

    public class Order
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; } = null!;

        [Required, MaxLength(50)]
        public string OrderCode { get; set; } = string.Empty;

        public OrderType OrderType { get; set; } = OrderType.ONLINE;

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(500)]
        public string? ShippingAddress { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.PENDING;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public Shipment? Shipment { get; set; }
        public Payment? Payment { get; set; }
        public Invoice? Invoice { get; set; }
    }
}
