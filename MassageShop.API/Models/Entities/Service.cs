using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MassageShop.API.Models.Entities
{
    public class Service
    {
        public int Id { get; set; }

        public int ServiceCategoryId { get; set; }

        [ForeignKey(nameof(ServiceCategoryId))]
        public ServiceCategory ServiceCategory { get; set; } = null!;

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>Thời gian thực hiện (phút)</summary>
        public int Duration { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
