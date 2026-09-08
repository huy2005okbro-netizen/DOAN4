using MassageShop.API.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace MassageShop.API.Models.DTOs.Payment
{
    public class CreatePaymentDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [MaxLength(100)]
        public string? TransactionCode { get; set; }
    }
}
