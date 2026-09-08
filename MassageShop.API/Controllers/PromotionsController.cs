using MassageShop.API.Interfaces;
using MassageShop.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MassageShop.API.Controllers
{
    [ApiController]
    [Route("api/promotions")]
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionService _service;

        public PromotionsController(IPromotionService service) => _service = service;

        /// <summary>Lấy danh sách khuyến mãi</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        /// <summary>Lấy khuyến mãi theo ID</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>Tạo khuyến mãi - ADMIN</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Promotion promotion)
        {
            var result = await _service.CreateAsync(promotion);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>Cập nhật khuyến mãi - ADMIN</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Promotion promotion)
        {
            var result = await _service.UpdateAsync(id, promotion);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>Xóa khuyến mãi - ADMIN</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result ? Ok(new { message = "Đã xóa khuyến mãi" }) : NotFound();
        }

        // ===== Vouchers =====

        /// <summary>Lấy danh sách voucher theo khuyến mãi</summary>
        [HttpGet("{promotionId}/vouchers")]
        public async Task<IActionResult> GetVouchers(int promotionId) =>
            Ok(await _service.GetVouchersByPromotionAsync(promotionId));

        /// <summary>Tạo voucher - ADMIN</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpPost("{promotionId}/vouchers")]
        public async Task<IActionResult> CreateVoucher(int promotionId, [FromBody] Voucher voucher)
        {
            voucher.PromotionId = promotionId;
            try
            {
                var result = await _service.CreateVoucherAsync(voucher);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Kiểm tra voucher hợp lệ</summary>
        [HttpGet("vouchers/validate")]
        public async Task<IActionResult> ValidateVoucher([FromQuery] string code, [FromQuery] decimal orderAmount)
        {
            var result = await _service.ValidateVoucherAsync(code, orderAmount);
            if (result == null)
                return BadRequest(new { message = "Voucher không hợp lệ, hết hạn hoặc không đủ điều kiện" });
            return Ok(result);
        }
    }
}
