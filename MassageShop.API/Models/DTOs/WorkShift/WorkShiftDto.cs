namespace MassageShop.API.Models.DTOs.WorkShift
{
    public class WorkShiftDto
    {
        public int EmployeeId { get; set; }
        public DateTime WorkDate { get; set; }
        public string ShiftType { get; set; } = "MORNING";
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int? RoomId { get; set; }
        public string Status { get; set; } = "WORKING";
        public string? Note { get; set; }
    }

    // A calendar shift can contain several employee-specific WorkShift rows.
    public class WorkShiftGroupDto
    {
        public List<int> Ids { get; set; } = new();
        public List<int> EmployeeIds { get; set; } = new();
        public DateTime WorkDate { get; set; }
        public string ShiftType { get; set; } = "MORNING";
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int? RoomId { get; set; }
        public string Status { get; set; } = "WORKING";
        public string? Note { get; set; }
    }

    public class WorkShiftResponseDto : WorkShiftDto
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; } = "";
        public string? Position { get; set; }
        public string? RoomNumber { get; set; }
    }
}
