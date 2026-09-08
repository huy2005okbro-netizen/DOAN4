using MassageShop.API.Interfaces;
using MassageShop.API.Models.DTOs.Payment;
using MassageShop.API.Models.Entities;
using MassageShop.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MassageShop.API.Controllers
{
    [ApiController]
    [Route("api/payments")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentsController(IPaymentService service) => _service = service;

        /// <summary>Lấy tất cả thanh toán - ADMIN, EMPLOYEE</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        /// <summary>Lấy thanh toán theo ID</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound(new { message = "Không tìm thấy thanh toán" }) : Ok(result);
        }

        /// <summary>Lấy thanh toán theo đơn hàng</summary>
        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetByOrder(int orderId)
        {
            var result = await _service.GetByOrderIdAsync(orderId);
            return result == null ? NotFound(new { message = "Đơn hàng chưa có thông tin thanh toán" }) : Ok(result);
        }

        /// <summary>Lấy thanh toán của đơn hàng thuộc về tôi - CUSTOMER</summary>
        [Authorize(Roles = "CUSTOMER")]
        [HttpGet("my/order/{orderId}")]
        public async Task<IActionResult> GetMyPaymentByOrder(int orderId,
            [FromServices] ICustomerService customerService,
            [FromServices] IOrderService orderService)
        {
            var customer = await customerService.GetByUserIdAsync(User.GetUserId());
            if (customer == null) return NotFound(new { message = "Không tìm thấy thông tin khách hàng" });

            var order = await orderService.GetByIdAsync(orderId);
            if (order == null) return NotFound(new { message = "Không tìm thấy đơn hàng" });
            if (order.CustomerId != customer.Id) return Forbid();

            var result = await _service.GetByOrderIdAsync(orderId);
            return result == null ? NotFound(new { message = "Đơn hàng chưa có thông tin thanh toán" }) : Ok(result);
        }

        /// <summary>Tạo thanh toán - ADMIN, EMPLOYEE</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Cập nhật trạng thái thanh toán - ADMIN, EMPLOYEE</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdatePaymentStatusRequest req)
        {
            var result = await _service.UpdateStatusAsync(id, req.Status);
            return result == null ? NotFound() : Ok(result);
        }
    }

    public record UpdatePaymentStatusRequest(PaymentStatus Status);
}
