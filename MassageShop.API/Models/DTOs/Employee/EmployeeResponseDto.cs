namespace MassageShop.API.Models.DTOs.Employee
{
    public class EmployeeResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Position { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsActive { get; set; }
    }
}
