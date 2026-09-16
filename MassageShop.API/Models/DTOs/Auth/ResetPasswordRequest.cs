using System.ComponentModel.DataAnnotations;

namespace MassageShop.API.Models.DTOs.Auth
{
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "OTP là bắt buộc")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
