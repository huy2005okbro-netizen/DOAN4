using System.ComponentModel.DataAnnotations;

namespace MassageShop.API.Models.DTOs.Shipment
{
    public class CreateShipmentDto
    {
        [Required]
        public int OrderId { get; set; }

        public int? ShippingMethodId { get; set; }

        [Required, MaxLength(100)]
        public string RecipientName { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string RecipientPhone { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string ShippingAddress { get; set; } = string.Empty;
    }
}
