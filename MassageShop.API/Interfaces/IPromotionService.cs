using MassageShop.API.Models.Entities;

namespace MassageShop.API.Interfaces
{
    public interface IPromotionService
    {
        Task<List<Promotion>> GetAllAsync();
        Task<Promotion?> GetByIdAsync(int id);
        Task<Promotion> CreateAsync(Promotion promotion);
        Task<Promotion?> UpdateAsync(int id, Promotion promotion);
        Task<bool> DeleteAsync(int id);

        // Voucher
        Task<List<Voucher>> GetVouchersByPromotionAsync(int promotionId);
        Task<Voucher> CreateVoucherAsync(Voucher voucher);
        Task<Voucher?> ValidateVoucherAsync(string code, decimal orderAmount);
    }
}
