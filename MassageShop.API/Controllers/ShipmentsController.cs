using MassageShop.API.Helpers;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.DTOs.Shipment;
using MassageShop.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MassageShop.API.Controllers
{
    [ApiController]
    [Route("api/shipments")]
    [Authorize]
    public class ShipmentsController : ControllerBase
    {
        private readonly IShipmentService _service;

        public ShipmentsController(IShipmentService service) => _service = service;
        /// <summary>Lấy tất cả vận đơn - ADMIN, EMPLOYEE</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        /// <summary>Lấy vận đơn theo ID</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound(new { message = "Không tìm thấy vận đơn" }) : Ok(result);
        }

        /// <summary>Lấy vận đơn theo đơn hàng</summary>
        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetByOrder(int orderId)
        {
            var result = await _service.GetByOrderIdAsync(orderId);
            return result == null ? NotFound(new { message = "Đơn hàng chưa có vận đơn" }) : Ok(result);
        }

        /// <summary>Khách hàng theo dõi vận đơn của đơn hàng mình - CUSTOMER</summary>
        [Authorize(Roles = "CUSTOMER")]
        [HttpGet("my/order/{orderId}")]
        public async Task<IActionResult> TrackMyShipment(int orderId,
            [FromServices] ICustomerService customerService,
            [FromServices] IOrderService orderService)
        {
            var customer = await customerService.GetByUserIdAsync(User.GetUserId());
            if (customer == null) return NotFound(new { message = "Không tìm thấy thông tin khách hàng" });

            var order = await orderService.GetByIdAsync(orderId);
            if (order == null) return NotFound(new { message = "Không tìm thấy đơn hàng" });
            if (order.CustomerId != customer.Id) return Forbid();

            var result = await _service.GetByOrderIdAsync(orderId);
            return result == null ? NotFound(new { message = "Đơn hàng chưa có vận đơn" }) : Ok(result);
        }

        /// <summary>Tạo vận đơn - ADMIN, EMPLOYEE</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShipmentDto dto)
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

        /// <summary>Cập nhật trạng thái vận đơn - ADMIN, EMPLOYEE</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateShipmentStatusRequest req)
        {
            var result = await _service.UpdateStatusAsync(id, req.Status);
            return result == null ? NotFound() : Ok(result);
        }
    }

    public record UpdateShipmentStatusRequest(ShipmentStatus Status);
}
