using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MassageShop.API.Models.Entities
{
    public class Employee
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [MaxLength(100)]
        public string? Position { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
