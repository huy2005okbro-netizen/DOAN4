using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MassageShop.API.Models.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public int ProductCategoryId { get; set; }

        [ForeignKey(nameof(ProductCategoryId))]
        public ProductCategory ProductCategory { get; set; } = null!;

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostPrice { get; set; }

        public int StockQuantity { get; set; } = 0;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(100)]
        public string? Brand { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
