using MassageShop.API.Data;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _db;

        public InventoryService(AppDbContext db) => _db = db;

        public async Task<List<InventoryTransaction>> GetAllAsync() =>
            await _db.InventoryTransactions.Include(t => t.Product).ToListAsync();

        public async Task<List<InventoryTransaction>> GetByProductIdAsync(int productId) =>
            await _db.InventoryTransactions
                .Include(t => t.Product)
                .Where(t => t.ProductId == productId)
                .ToListAsync();

        public async Task<InventoryTransaction> ImportAsync(int productId, int quantity, string? note)
        {
            var product = await _db.Products.FindAsync(productId)
                ?? throw new KeyNotFoundException("Sản phẩm không tồn tại");

            if (quantity <= 0)
                throw new InvalidOperationException("Số lượng nhập phải lớn hơn 0");

            product.StockQuantity += quantity;

            var transaction = new InventoryTransaction
            {
                ProductId = productId,
                Type = InventoryTransactionType.IMPORT,
                Quantity = quantity,
                ReferenceType = "MANUAL",
                Note = note
            };
            _db.InventoryTransactions.Add(transaction);
            await _db.SaveChangesAsync();
            return transaction;
        }

        public async Task<InventoryTransaction> AdjustAsync(int productId, int quantity, string? note)
        {
            var product = await _db.Products.FindAsync(productId)
                ?? throw new KeyNotFoundException("Sản phẩm không tồn tại");

            var newQty = product.StockQuantity + quantity;
            if (newQty < 0)
                throw new InvalidOperationException("Tồn kho không thể âm");

            product.StockQuantity = newQty;

            var transaction = new InventoryTransaction
            {
                ProductId = productId,
                Type = InventoryTransactionType.ADJUSTMENT,
                Quantity = quantity,
                ReferenceType = "MANUAL",
                Note = note
            };
            _db.InventoryTransactions.Add(transaction);
            await _db.SaveChangesAsync();
            return transaction;
        }
    }
}
