using MassageShop.API.Data;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.DTOs.Review;
using MassageShop.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Services
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _db;

        public ReviewService(AppDbContext db) => _db = db;

        public async Task<List<ReviewResponseDto>> GetAllAsync() =>
            await QueryWithIncludes().Select(r => MapToDto(r)).ToListAsync();

        public async Task<List<ReviewResponseDto>> GetByProductIdAsync(int productId) =>
            await QueryWithIncludes()
                .Where(r => r.ProductId == productId && r.IsApproved)
                .Select(r => MapToDto(r))
                .ToListAsync();

        public async Task<List<ReviewResponseDto>> GetByServiceIdAsync(int serviceId) =>
            await QueryWithIncludes()
                .Where(r => r.ServiceId == serviceId && r.IsApproved)
                .Select(r => MapToDto(r))
                .ToListAsync();

        public async Task<List<ReviewResponseDto>> GetByCustomerIdAsync(int customerId) =>
            await QueryWithIncludes()
                .Where(r => r.CustomerId == customerId)
                .Select(r => MapToDto(r))
                .ToListAsync();

        public async Task<ReviewResponseDto> CreateAsync(int customerId, CreateReviewDto dto)
        {
            if (dto.ProductId == null && dto.ServiceId == null)
                throw new InvalidOperationException("Phải chọn sản phẩm hoặc dịch vụ để đánh giá");

            if (dto.ProductId != null && dto.ServiceId != null)
                throw new InvalidOperationException("Chỉ được đánh giá một đối tượng");

            // Kiểm tra đã mua sản phẩm chưa
            if (dto.ProductId.HasValue)
            {
                var hasBought = await _db.OrderItems
                    .Include(oi => oi.Order)
                    .AnyAsync(oi => oi.ProductId == dto.ProductId &&
                                    oi.Order.CustomerId == customerId &&
                                    oi.Order.Status == OrderStatus.COMPLETED);

                if (!hasBought)
                    throw new InvalidOperationException("Bạn chỉ có thể đánh giá sản phẩm đã mua");
            }

            // Kiểm tra đã sử dụng dịch vụ chưa
            if (dto.ServiceId.HasValue)
            {
                var hasUsed = await _db.Appointments
                    .AnyAsync(a => a.ServiceId == dto.ServiceId &&
                                   a.CustomerId == customerId &&
                                   a.Status == AppointmentStatus.COMPLETED);

                if (!hasUsed)
                    throw new InvalidOperationException("Bạn chỉ có thể đánh giá dịch vụ đã sử dụng");
            }

            var review = new Review
            {
                CustomerId = customerId,
                ProductId = dto.ProductId,
                ServiceId = dto.ServiceId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };
            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();

            return (await QueryWithIncludes()
                .Where(r => r.Id == review.Id)
                .Select(r => MapToDto(r))
                .FirstAsync());
        }

        public async Task<ReviewResponseDto?> ApproveAsync(int id)
        {
            var review = await _db.Reviews.FindAsync(id);
            if (review == null) return null;
            review.IsApproved = true;
            await _db.SaveChangesAsync();
            return await QueryWithIncludes()
                .Where(r => r.Id == id)
                .Select(r => MapToDto(r))
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var review = await _db.Reviews.FindAsync(id);
            if (review == null) return false;
            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();
            return true;
        }

        private IQueryable<Review> QueryWithIncludes() =>
            _db.Reviews
                .Include(r => r.Customer).ThenInclude(c => c.User)
                .Include(r => r.Product)
                .Include(r => r.Service);

        private static ReviewResponseDto MapToDto(Review r) => new()
        {
            Id = r.Id,
            CustomerId = r.CustomerId,
            CustomerName = r.Customer.User.FullName,
            ProductId = r.ProductId,
            ProductName = r.Product?.Name,
            ServiceId = r.ServiceId,
            ServiceName = r.Service?.Name,
            Rating = r.Rating,
            Comment = r.Comment,
            IsApproved = r.IsApproved,
            CreatedAt = r.CreatedAt
        };
    }
}
