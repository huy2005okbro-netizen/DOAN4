using System.ComponentModel.DataAnnotations;

namespace MassageShop.API.Models.DTOs.Review
{
    public class CreateReviewDto
    {
        /// <summary>Điền ProductId hoặc ServiceId, không điền cả hai</summary>
        public int? ProductId { get; set; }

        public int? ServiceId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating phải từ 1 đến 5")]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
    }
}
