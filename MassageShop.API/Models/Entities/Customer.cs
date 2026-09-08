using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MassageShop.API.Models.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [MaxLength(255)]
        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }

        public int LoyaltyPoints { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public Cart? Cart { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
