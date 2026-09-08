using System.ComponentModel.DataAnnotations;

namespace MassageShop.API.Models.DTOs.Product
{
    public class ProductUpdateDto
    {
        public int? ProductCategoryId { get; set; }

        [MaxLength(150)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? Price { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? CostPrice { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(100)]
        public string? Brand { get; set; }

        public bool? IsActive { get; set; }
    }
}
