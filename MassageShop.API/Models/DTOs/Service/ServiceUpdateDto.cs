using System.ComponentModel.DataAnnotations;

namespace MassageShop.API.Models.DTOs.Service
{
    public class ServiceUpdateDto
    {
        public int? ServiceCategoryId { get; set; }

        [MaxLength(150)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Range(1, int.MaxValue)]
        public int? Duration { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? Price { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool? IsActive { get; set; }
    }
}
