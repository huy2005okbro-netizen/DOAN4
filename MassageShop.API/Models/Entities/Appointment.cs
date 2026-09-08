using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MassageShop.API.Models.Entities
{
    public enum AppointmentStatus
    {
        PENDING,
        CONFIRMED,
        CHECKED_IN,
        IN_PROGRESS,
        COMPLETED,
        CANCELLED
    }

    public class Appointment
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; } = null!;

        public int ServiceId { get; set; }

        [ForeignKey(nameof(ServiceId))]
        public Service Service { get; set; } = null!;

        public int? EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public Employee? Employee { get; set; }

        public int? RoomId { get; set; }

        [ForeignKey(nameof(RoomId))]
        public Room? Room { get; set; }

        public DateTime AppointmentDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.PENDING;

        [MaxLength(500)]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
