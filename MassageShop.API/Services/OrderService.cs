using MassageShop.API.Data;
using MassageShop.API.Helpers;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.DTOs.Order;
using MassageShop.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;

        public OrderService(AppDbContext db) => _db = db;

        public async Task<List<OrderResponseDto>> GetAllAsync() =>
            await QueryWithIncludes().Select(o => MapToDto(o)).ToListAsync();

        public async Task<List<OrderResponseDto>> GetByCustomerIdAsync(int customerId) =>
            await QueryWithIncludes()
                .Where(o => o.CustomerId == customerId)
                .Select(o => MapToDto(o))
                .ToListAsync();

        public async Task<OrderResponseDto?> GetByIdAsync(int id)
        {
            var o = await QueryWithIncludes().FirstOrDefaultAsync(o => o.Id == id);
            return o == null ? null : MapToDto(o);
        }

        public async Task<OrderResponseDto> CreateAsync(int customerId, CreateOrderDto dto)
        {
            if (!dto.Items.Any())
                throw new InvalidOperationException("Đơn hàng phải có ít nhất một sản phẩm");

            // Validate và tính tiền
            decimal subTotal = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in dto.Items)
            {
                var product = await _db.Products.FindAsync(item.ProductId)
                    ?? throw new KeyNotFoundException($"Sản phẩm ID {item.ProductId} không tồn tại");

                if (!product.IsActive)
                    throw new InvalidOperationException($"Sản phẩm '{product.Name}' không còn kinh doanh");

                if (product.StockQuantity < item.Quantity)
                    throw new InvalidOperationException($"Sản phẩm '{product.Name}' không đủ tồn kho (còn {product.StockQuantity})");

                var total = product.Price * item.Quantity;
                subTotal += total;
                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    TotalPrice = total
                });
            }

            // Áp dụng voucher
            decimal discount = 0;
            if (!string.IsNullOrWhiteSpace(dto.VoucherCode))
            {
                var voucher = await _db.Vouchers
                    .Include(v => v.Promotion)
                    .FirstOrDefaultAsync(v => v.Code == dto.VoucherCode);

                if (voucher == null)
                    throw new InvalidOperationException("Mã voucher không tồn tại");
                if (voucher.UsedCount >= voucher.UsageLimit)
                    throw new InvalidOperationException("Mã voucher đã hết lượt sử dụng");
                if (subTotal < voucher.MinimumOrderAmount)
                    throw new InvalidOperationException($"Đơn hàng tối thiểu {voucher.MinimumOrderAmount:N0}đ để dùng voucher này");
                if (!voucher.Promotion.IsActive || DateTime.UtcNow > voucher.Promotion.EndDate)
                    throw new InvalidOperationException("Mã voucher đã hết hạn");

                discount = voucher.Promotion.DiscountType == DiscountType.PERCENTAGE
                    ? subTotal * voucher.Promotion.DiscountValue / 100
                    : voucher.Promotion.DiscountValue;

                voucher.UsedCount++;
            }

            // Tính phí ship (chỉ ONLINE)
            decimal shippingFee = 0;
            if (dto.OrderType == OrderType.ONLINE)
            {
                if (dto.ShippingMethodId.HasValue)
                {
                    var method = await _db.ShippingMethods.FindAsync(dto.ShippingMethodId.Value);
                    shippingFee = method?.Fee ?? 0;
                }
            }

            var order = new Order
            {
                CustomerId = customerId,
                OrderCode = OrderCodeHelper.Generate(),
                OrderType = dto.OrderType,
                SubTotal = subTotal,
                DiscountAmount = discount,
                ShippingFee = shippingFee,
                TotalAmount = subTotal - discount + shippingFee,
                ShippingAddress = dto.ShippingAddress,
                Note = dto.Note,
                OrderItems = orderItems
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            // Tạo Payment
            var payment = new Payment
            {
                OrderId = order.Id,
                PaymentMethod = dto.PaymentMethod,
                Amount = order.TotalAmount
            };
            _db.Payments.Add(payment);

            // Với IN_STORE: xác nhận luôn và trừ tồn kho
            if (dto.OrderType == OrderType.IN_STORE)
            {
                order.Status = OrderStatus.CONFIRMED;
                await DeductStockAsync(orderItems);
            }

            await _db.SaveChangesAsync();
            return (await GetByIdAsync(order.Id))!;
        }

        public async Task<OrderResponseDto?> UpdateStatusAsync(int id, OrderStatus status)
        {
            var order = await _db.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return null;

            // Khi ONLINE chuyển sang CONFIRMED thì trừ tồn kho
            if (order.OrderType == OrderType.ONLINE
                && order.Status == OrderStatus.PENDING
                && status == OrderStatus.CONFIRMED)
            {
                await DeductStockAsync(order.OrderItems.ToList());
            }

            order.Status = status;
            await _db.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> CancelAsync(int id, int requesterId, string requesterRole)
        {
            var order = await _db.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return false;

            if (requesterRole == "CUSTOMER" && order.Customer.UserId != requesterId)
                throw new UnauthorizedAccessException("Không có quyền hủy đơn hàng này");

            // Nếu đã trừ kho (CONFIRMED trở đi) thì hoàn lại
            if (order.Status == OrderStatus.CONFIRMED || order.Status == OrderStatus.PREPARING)
            {
                await RestoreStockAsync(order.OrderItems.ToList());
            }

            order.Status = OrderStatus.CANCELLED;
            await _db.SaveChangesAsync();
            return true;
        }

        // ===== Helpers =====

        private async Task DeductStockAsync(List<OrderItem> items)
        {
            foreach (var item in items)
            {
                var product = await _db.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity -= item.Quantity;
                    _db.InventoryTransactions.Add(new InventoryTransaction
                    {
                        ProductId = item.ProductId,
                        Type = InventoryTransactionType.SALE,
                        Quantity = -item.Quantity,
                        ReferenceType = "ORDER",
                        Note = "Bán hàng"
                    });
                }
            }
        }

        private async Task RestoreStockAsync(List<OrderItem> items)
        {
            foreach (var item in items)
            {
                var product = await _db.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                    _db.InventoryTransactions.Add(new InventoryTransaction
                    {
                        ProductId = item.ProductId,
                        Type = InventoryTransactionType.RETURN,
                        Quantity = item.Quantity,
                        ReferenceType = "ORDER",
                        Note = "Hủy đơn - hoàn kho"
                    });
                }
            }
        }

        private IQueryable<Order> QueryWithIncludes() =>
            _db.Orders
                .Include(o => o.Customer).ThenInclude(c => c.User)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product);

        private static OrderResponseDto MapToDto(Order o) => new()
        {
            Id = o.Id,
            CustomerId = o.CustomerId,
            CustomerName = o.Customer.User.FullName,
            OrderCode = o.OrderCode,
            OrderType = o.OrderType.ToString(),
            SubTotal = o.SubTotal,
            DiscountAmount = o.DiscountAmount,
            ShippingFee = o.ShippingFee,
            TotalAmount = o.TotalAmount,
            ShippingAddress = o.ShippingAddress,
            Note = o.Note,
            Status = o.Status,
            CreatedAt = o.CreatedAt,
            Items = o.OrderItems.Select(oi => new OrderItemResponseDto
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                ProductName = oi.Product.Name,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                TotalPrice = oi.TotalPrice
            }).ToList()
        };
    }
}
