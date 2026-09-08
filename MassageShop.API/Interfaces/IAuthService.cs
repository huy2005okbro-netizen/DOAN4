using MassageShop.API.Models.DTOs.Auth;

namespace MassageShop.API.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> RegisterAsync(RegisterRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<LoginResponse> GetMeAsync(int userId);
    }
}
