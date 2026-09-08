using MassageShop.API.Models.DTOs.Review;

namespace MassageShop.API.Interfaces
{
    public interface IReviewService
    {
        Task<List<ReviewResponseDto>> GetAllAsync();
        Task<List<ReviewResponseDto>> GetByProductIdAsync(int productId);
        Task<List<ReviewResponseDto>> GetByServiceIdAsync(int serviceId);
        Task<List<ReviewResponseDto>> GetByCustomerIdAsync(int customerId);
        Task<ReviewResponseDto> CreateAsync(int customerId, CreateReviewDto dto);
        Task<ReviewResponseDto?> ApproveAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
