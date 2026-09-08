using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MassageShop.API.Models.Entities
{
    public class Review
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; } = null!;

        /// <summary>Null nếu đánh giá dịch vụ</summary>
        public int? ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        /// <summary>Null nếu đánh giá sản phẩm</summary>
        public int? ServiceId { get; set; }

        [ForeignKey(nameof(ServiceId))]
        public Service? Service { get; set; }

        /// <summary>Rating từ 1 đến 5</summary>
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsApproved { get; set; } = false;
    }
}
