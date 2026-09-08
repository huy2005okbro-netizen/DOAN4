using MassageShop.API.Helpers;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.DTOs.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MassageShop.API.Controllers
{
    [ApiController]
    [Route("api/carts")]
    [Authorize(Roles = "CUSTOMER")]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ICustomerService _customerService;

        public CartsController(ICartService cartService, ICustomerService customerService)
        {
            _cartService = cartService;
            _customerService = customerService;
        }

        /// <summary>Lấy giỏ hàng của tôi</summary>
        [HttpGet]
        public async Task<IActionResult> GetMyCart()
        {
            var customer = await _customerService.GetByUserIdAsync(User.GetUserId());
            if (customer == null) return NotFound(new { message = "Không tìm thấy thông tin khách hàng" });
            return Ok(await _cartService.GetCartAsync(customer.Id));
        }

        /// <summary>Thêm sản phẩm vào giỏ hàng</summary>
        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemDto dto)
        {
            var customer = await _customerService.GetByUserIdAsync(User.GetUserId());
            if (customer == null) return NotFound(new { message = "Không tìm thấy thông tin khách hàng" });

            try
            {
                var result = await _cartService.AddItemAsync(customer.Id, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Cập nhật số lượng sản phẩm trong giỏ</summary>
        [HttpPut("items/{cartItemId}")]
        public async Task<IActionResult> UpdateItem(int cartItemId, [FromBody] UpdateCartItemDto dto)
        {
            var customer = await _customerService.GetByUserIdAsync(User.GetUserId());
            if (customer == null) return NotFound(new { message = "Không tìm thấy thông tin khách hàng" });

            try
            {
                var result = await _cartService.UpdateItemAsync(customer.Id, cartItemId, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>Xóa sản phẩm khỏi giỏ hàng</summary>
        [HttpDelete("items/{cartItemId}")]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var customer = await _customerService.GetByUserIdAsync(User.GetUserId());
            if (customer == null) return NotFound(new { message = "Không tìm thấy thông tin khách hàng" });

            try
            {
                var result = await _cartService.RemoveItemAsync(customer.Id, cartItemId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>Xóa toàn bộ giỏ hàng</summary>
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var customer = await _customerService.GetByUserIdAsync(User.GetUserId());
            if (customer == null) return NotFound(new { message = "Không tìm thấy thông tin khách hàng" });

            await _cartService.ClearCartAsync(customer.Id);
            return Ok(new { message = "Đã xóa giỏ hàng" });
        }
    }
}
