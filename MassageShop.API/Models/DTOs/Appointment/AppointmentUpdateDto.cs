using MassageShop.API.Models.Entities;

namespace MassageShop.API.Models.DTOs.Appointment
{
    public class AppointmentUpdateDto
    {
        public int? EmployeeId { get; set; }
        public int? RoomId { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public AppointmentStatus? Status { get; set; }
        public string? Note { get; set; }
    }
}
