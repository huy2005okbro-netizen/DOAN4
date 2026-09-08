using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MassageShop.API.Models.Entities
{
    public enum PaymentMethod
    {
        CASH,
        BANK_TRANSFER,
        COD
    }

    public enum PaymentStatus
    {
        PENDING,
        PAID,
        FAILED,
        REFUNDED
    }

    public class Payment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; } = null!;

        public PaymentMethod PaymentMethod { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.PENDING;

        [MaxLength(100)]
        public string? TransactionCode { get; set; }

        public DateTime? PaidAt { get; set; }
    }
}
