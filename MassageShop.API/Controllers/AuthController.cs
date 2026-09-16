using MassageShop.API.Helpers;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MassageShop.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) => _authService = authService;

        /// <summary>Đăng ký tài khoản khách hàng</summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Đăng nhập (tất cả vai trò)</summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>Lấy thông tin người dùng hiện tại</summary>
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.GetUserId();
            var result = await _authService.GetMeAsync(userId);
            return Ok(result);
        }

        /// <summary>Quên mật khẩu — gửi OTP về email</summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(new { message = "Email là bắt buộc" });

            // Luôn trả về OK để không lộ thông tin email có tồn tại hay không
            await _authService.ForgotPasswordAsync(request.Email);
            return Ok(new { message = "Nếu email tồn tại, hướng dẫn đặt lại mật khẩu đã được gửi." });
        }

        /// <summary>Đặt lại mật khẩu bằng OTP</summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest(new { message = "Token và mật khẩu mới là bắt buộc" });

            if (request.NewPassword.Length < 6)
                return BadRequest(new { message = "Mật khẩu mới tối thiểu 6 ký tự" });

            var success = await _authService.ResetPasswordAsync(request.Token, request.NewPassword);
            if (!success)
                return BadRequest(new { message = "OTP không hợp lệ hoặc đã hết hạn" });

            return Ok(new { message = "Đặt lại mật khẩu thành công" });
        }

        /// <summary>Đổi mật khẩu (khi đã đăng nhập)</summary>
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmPassword)
                return BadRequest(new { message = "Mật khẩu xác nhận không khớp" });

            if (request.NewPassword.Length < 6)
                return BadRequest(new { message = "Mật khẩu mới tối thiểu 6 ký tự" });

            try
            {
                var success = await _authService.ChangePasswordAsync(
                    User.GetUserId(), request.CurrentPassword, request.NewPassword);

                if (!success)
                    return NotFound(new { message = "Không tìm thấy tài khoản" });

                return Ok(new { message = "Đổi mật khẩu thành công" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
