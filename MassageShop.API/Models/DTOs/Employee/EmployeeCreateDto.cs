using System.ComponentModel.DataAnnotations;

namespace MassageShop.API.Models.DTOs.Employee
{
    public class EmployeeCreateDto
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Position { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
    }
}
