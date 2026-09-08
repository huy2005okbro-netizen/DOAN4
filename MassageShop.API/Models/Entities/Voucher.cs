using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MassageShop.API.Models.Entities
{
    public class Voucher
    {
        public int Id { get; set; }

        public int PromotionId { get; set; }

        [ForeignKey(nameof(PromotionId))]
        public Promotion Promotion { get; set; } = null!;

        [Required, MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        public int UsageLimit { get; set; }

        public int UsedCount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal MinimumOrderAmount { get; set; } = 0;
    }
}
