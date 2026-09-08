using MassageShop.API.Models.Entities;

namespace MassageShop.API.Models.DTOs.Payment
{
    public class PaymentResponseDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public string? TransactionCode { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
