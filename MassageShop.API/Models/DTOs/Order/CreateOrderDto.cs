using MassageShop.API.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace MassageShop.API.Models.DTOs.Order
{
    public class CreateOrderDto
    {
        [Required]
        public OrderType OrderType { get; set; }

        public List<CreateOrderItemDto> Items { get; set; } = new();

        public string? VoucherCode { get; set; }

        [MaxLength(500)]
        public string? ShippingAddress { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        // Chỉ dùng cho ONLINE
        public int? ShippingMethodId { get; set; }

        // Chỉ dùng cho ONLINE
        public string? RecipientName { get; set; }

        // Chỉ dùng cho ONLINE
        public string? RecipientPhone { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }
    }

    public class CreateOrderItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }
    }
}
