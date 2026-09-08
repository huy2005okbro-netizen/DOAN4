using System.ComponentModel.DataAnnotations;

namespace MassageShop.API.Models.Entities
{
    public enum RoomStatus
    {
        AVAILABLE,
        OCCUPIED,
        MAINTENANCE
    }

    public class Room
    {
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string RoomNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? RoomType { get; set; }

        public int Capacity { get; set; } = 1;

        public RoomStatus Status { get; set; } = RoomStatus.AVAILABLE;

        // Navigation
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
