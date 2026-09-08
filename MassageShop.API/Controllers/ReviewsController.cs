using MassageShop.API.Helpers;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.DTOs.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MassageShop.API.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _service;
        private readonly ICustomerService _customerService;

        public ReviewsController(IReviewService service, ICustomerService customerService)
        {
            _service = service;
            _customerService = customerService;
        }

        /// <summary>Lấy tất cả đánh giá - ADMIN</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        /// <summary>Lấy đánh giá của sản phẩm - Public</summary>
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProduct(int productId) =>
            Ok(await _service.GetByProductIdAsync(productId));

        /// <summary>Lấy đánh giá của dịch vụ - Public</summary>
        [HttpGet("service/{serviceId}")]
        public async Task<IActionResult> GetByService(int serviceId) =>
            Ok(await _service.GetByServiceIdAsync(serviceId));

        /// <summary>Tạo đánh giá - CUSTOMER</summary>
        [Authorize(Roles = "CUSTOMER")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
        {
            var customer = await _customerService.GetByUserIdAsync(User.GetUserId());
            if (customer == null) return NotFound(new { message = "Không tìm thấy thông tin khách hàng" });

            try
            {
                var result = await _service.CreateAsync(customer.Id, dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Xem đánh giá của tôi - CUSTOMER</summary>
        [Authorize(Roles = "CUSTOMER")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMy()
        {
            var customer = await _customerService.GetByUserIdAsync(User.GetUserId());
            if (customer == null) return NotFound(new { message = "Không tìm thấy thông tin khách hàng" });
            var mine = await _service.GetByCustomerIdAsync(customer.Id);
            return Ok(mine);
        }

        /// <summary>Duyệt đánh giá - ADMIN</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpPatch("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var result = await _service.ApproveAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>Xóa đánh giá - ADMIN</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result ? Ok(new { message = "Đã xóa đánh giá" }) : NotFound();
        }
    }
}
