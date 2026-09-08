using System.ComponentModel.DataAnnotations;

namespace MassageShop.API.Models.DTOs.Appointment
{
    public class AppointmentCreateDto
    {
        [Required]
        public int ServiceId { get; set; }

        public int? EmployeeId { get; set; }

        public int? RoomId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
