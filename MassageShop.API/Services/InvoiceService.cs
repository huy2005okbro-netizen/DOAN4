using MassageShop.API.Data;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly AppDbContext _db;

        public InvoiceService(AppDbContext db) => _db = db;

        public async Task<List<Invoice>> GetAllAsync() =>
            await _db.Invoices.Include(i => i.Order).ToListAsync();

        public async Task<Invoice?> GetByIdAsync(int id) =>
            await _db.Invoices.Include(i => i.Order).FirstOrDefaultAsync(i => i.Id == id);

        public async Task<Invoice?> GetByOrderIdAsync(int orderId) =>
            await _db.Invoices.Include(i => i.Order).FirstOrDefaultAsync(i => i.OrderId == orderId);

        public async Task<Invoice> GenerateAsync(int orderId)
        {
            if (await _db.Invoices.AnyAsync(i => i.OrderId == orderId))
                throw new InvalidOperationException("Hóa đơn cho đơn hàng này đã tồn tại");

            var order = await _db.Orders.FindAsync(orderId)
                ?? throw new KeyNotFoundException("Đơn hàng không tồn tại");

            var invoice = new Invoice
            {
                OrderId = orderId,
                InvoiceCode = $"INV{DateTime.UtcNow:yyMMddHHmmss}{new Random().Next(100, 999)}",
                SubTotal = order.SubTotal,
                DiscountAmount = order.DiscountAmount,
                ShippingFee = order.ShippingFee,
                TotalAmount = order.TotalAmount
            };

            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync();
            return invoice;
        }
    }
}
