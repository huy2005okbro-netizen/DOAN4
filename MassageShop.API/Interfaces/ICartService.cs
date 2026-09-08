using MassageShop.API.Models.DTOs.Cart;

namespace MassageShop.API.Interfaces
{
    public interface ICartService
    {
        Task<CartResponseDto> GetCartAsync(int customerId);
        Task<CartResponseDto> AddItemAsync(int customerId, AddCartItemDto dto);
        Task<CartResponseDto> UpdateItemAsync(int customerId, int cartItemId, UpdateCartItemDto dto);
        Task<CartResponseDto> RemoveItemAsync(int customerId, int cartItemId);
        Task ClearCartAsync(int customerId);
    }
}
