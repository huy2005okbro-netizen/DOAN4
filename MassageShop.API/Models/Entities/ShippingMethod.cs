using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MassageShop.API.Models.Entities
{
    public class ShippingMethod
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Fee { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    }
}
