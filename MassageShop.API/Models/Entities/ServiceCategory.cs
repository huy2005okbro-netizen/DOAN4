using System.ComponentModel.DataAnnotations;

namespace MassageShop.API.Models.Entities
{
    public class ServiceCategory
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Service> Services { get; set; } = new List<Service>();
    }
}
