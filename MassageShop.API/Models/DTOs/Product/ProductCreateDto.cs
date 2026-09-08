using System.ComponentModel.DataAnnotations;

namespace MassageShop.API.Models.DTOs.Product
{
    public class ProductCreateDto
    {
        [Required]
        public int ProductCategoryId { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue)]
        public decimal CostPrice { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; } = 0;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(100)]
        public string? Brand { get; set; }
    }
}
