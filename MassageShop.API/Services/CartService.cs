using MassageShop.API.Data;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.DTOs.Cart;
using MassageShop.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _db;

        public CartService(AppDbContext db) => _db = db;

        public async Task<CartResponseDto> GetCartAsync(int customerId)
        {
            var cart = await GetOrCreateCartAsync(customerId);
            return MapToDto(cart);
        }

        public async Task<CartResponseDto> AddItemAsync(int customerId, AddCartItemDto dto)
        {
            var product = await _db.Products.FindAsync(dto.ProductId)
                ?? throw new KeyNotFoundException("Sản phẩm không tồn tại");

            if (!product.IsActive)
                throw new InvalidOperationException("Sản phẩm không còn kinh doanh");

            var cart = await GetOrCreateCartAsync(customerId);

            var existing = cart.CartItems.FirstOrDefault(ci => ci.ProductId == dto.ProductId);
            if (existing != null)
            {
                existing.Quantity += dto.Quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UnitPrice = product.Price
                });
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return MapToDto(await GetCartWithItemsAsync(cart.Id));
        }

        public async Task<CartResponseDto> UpdateItemAsync(int customerId, int cartItemId, UpdateCartItemDto dto)
        {
            var cart = await GetOrCreateCartAsync(customerId);
            var item = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId)
                ?? throw new KeyNotFoundException("Không tìm thấy mục giỏ hàng");

            item.Quantity = dto.Quantity;
            cart.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return MapToDto(await GetCartWithItemsAsync(cart.Id));
        }

        public async Task<CartResponseDto> RemoveItemAsync(int customerId, int cartItemId)
        {
            var cart = await GetOrCreateCartAsync(customerId);
            var item = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId)
                ?? throw new KeyNotFoundException("Không tìm thấy mục giỏ hàng");

            _db.CartItems.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return MapToDto(await GetCartWithItemsAsync(cart.Id));
        }

        public async Task ClearCartAsync(int customerId)
        {
            var cart = await _db.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart != null)
            {
                _db.CartItems.RemoveRange(cart.CartItems);
                cart.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
        }

        // ===== Helpers =====

        private async Task<Cart> GetOrCreateCartAsync(int customerId)
        {
            var cart = await _db.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
            {
                cart = new Cart { CustomerId = customerId };
                _db.Carts.Add(cart);
                await _db.SaveChangesAsync();
            }

            return cart;
        }

        private async Task<Cart> GetCartWithItemsAsync(int cartId) =>
            await _db.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstAsync(c => c.Id == cartId);

        private static CartResponseDto MapToDto(Cart cart) => new()
        {
            Id = cart.Id,
            CustomerId = cart.CustomerId,
            UpdatedAt = cart.UpdatedAt,
            Items = cart.CartItems.Select(ci => new CartItemResponseDto
            {
                Id = ci.Id,
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                ProductImage = ci.Product.ImageUrl,
                Quantity = ci.Quantity,
                UnitPrice = ci.UnitPrice,
                SubTotal = ci.UnitPrice * ci.Quantity
            }).ToList(),
            TotalAmount = cart.CartItems.Sum(ci => ci.UnitPrice * ci.Quantity)
        };
    }
}
