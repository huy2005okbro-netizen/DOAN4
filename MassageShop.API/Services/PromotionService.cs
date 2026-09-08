using MassageShop.API.Data;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly AppDbContext _db;

        public PromotionService(AppDbContext db) => _db = db;

        public async Task<List<Promotion>> GetAllAsync() =>
            await _db.Promotions.Include(p => p.Vouchers).ToListAsync();

        public async Task<Promotion?> GetByIdAsync(int id) =>
            await _db.Promotions.Include(p => p.Vouchers).FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Promotion> CreateAsync(Promotion promotion)
        {
            _db.Promotions.Add(promotion);
            await _db.SaveChangesAsync();
            return promotion;
        }

        public async Task<Promotion?> UpdateAsync(int id, Promotion updated)
        {
            var promo = await _db.Promotions.FindAsync(id);
            if (promo == null) return null;

            promo.Name = updated.Name;
            promo.Description = updated.Description;
            promo.DiscountType = updated.DiscountType;
            promo.DiscountValue = updated.DiscountValue;
            promo.StartDate = updated.StartDate;
            promo.EndDate = updated.EndDate;
            promo.IsActive = updated.IsActive;

            await _db.SaveChangesAsync();
            return promo;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var promo = await _db.Promotions.FindAsync(id);
            if (promo == null) return false;
            _db.Promotions.Remove(promo);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<Voucher>> GetVouchersByPromotionAsync(int promotionId) =>
            await _db.Vouchers.Where(v => v.PromotionId == promotionId).ToListAsync();

        public async Task<Voucher> CreateVoucherAsync(Voucher voucher)
        {
            if (await _db.Vouchers.AnyAsync(v => v.Code == voucher.Code))
                throw new InvalidOperationException("Mã voucher đã tồn tại");

            _db.Vouchers.Add(voucher);
            await _db.SaveChangesAsync();
            return voucher;
        }

        public async Task<Voucher?> ValidateVoucherAsync(string code, decimal orderAmount)
        {
            var voucher = await _db.Vouchers
                .Include(v => v.Promotion)
                .FirstOrDefaultAsync(v => v.Code == code);

            if (voucher == null) return null;
            if (voucher.UsedCount >= voucher.UsageLimit) return null;
            if (orderAmount < voucher.MinimumOrderAmount) return null;
            if (!voucher.Promotion.IsActive) return null;
            if (DateTime.UtcNow < voucher.Promotion.StartDate || DateTime.UtcNow > voucher.Promotion.EndDate) return null;

            return voucher;
        }
    }
}
