namespace MassageShop.API.Models.DTOs.Service
{
    public class ServiceResponseDto
    {
        public int Id { get; set; }
        public int ServiceCategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Duration { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
