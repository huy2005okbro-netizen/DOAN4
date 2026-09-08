using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MassageShop.API.Models.Entities
{
    public enum DiscountType
    {
        PERCENTAGE,
        FIXED_AMOUNT
    }

    public class Promotion
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public DiscountType DiscountType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountValue { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
    }
}
