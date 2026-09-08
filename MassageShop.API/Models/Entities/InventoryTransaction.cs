using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MassageShop.API.Models.Entities
{
    public enum InventoryTransactionType
    {
        IMPORT,
        EXPORT,
        ADJUSTMENT,
        SALE,
        RETURN
    }

    public class InventoryTransaction
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;

        public InventoryTransactionType Type { get; set; }

        public int Quantity { get; set; }

        [MaxLength(100)]
        public string? ReferenceType { get; set; }

        public int? ReferenceId { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
