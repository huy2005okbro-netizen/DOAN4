using System.ComponentModel.DataAnnotations;

namespace MassageShop.API.Models.DTOs.Employee
{
    public class EmployeeUpdateDto
    {
        [MaxLength(100)]
        public string? FullName { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Position { get; set; }

        public DateTime? StartDate { get; set; }

        public bool? IsActive { get; set; }
    }
}
