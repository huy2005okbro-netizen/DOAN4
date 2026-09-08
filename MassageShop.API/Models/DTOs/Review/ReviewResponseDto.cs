namespace MassageShop.API.Models.DTOs.Review
{
    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
