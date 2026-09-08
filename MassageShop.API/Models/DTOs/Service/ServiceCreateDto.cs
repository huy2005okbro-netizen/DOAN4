using System.ComponentModel.DataAnnotations;

namespace MassageShop.API.Models.DTOs.Service
{
    public class ServiceCreateDto
    {
        [Required]
        public int ServiceCategoryId { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Thời gian phải lớn hơn 0")]
        public int Duration { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }
    }
}
