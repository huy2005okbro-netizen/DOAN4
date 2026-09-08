using MassageShop.API.Helpers;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.DTOs.Order;
using MassageShop.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MassageShop.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;

        public OrdersController(IOrderService orderService, ICustomerService customerService)
        {
            _orderService = orderService;
            _customerService = customerService;
        }

        /// <summary>Lấy tất cả đơn hàng - ADMIN, EMPLOYEE. Filter tùy chọn theo status, orderType</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] OrderStatus? status,
            [FromQuery] OrderType? orderType)
        {
            var list = await _orderService.GetAllAsync();

            if (status.HasValue)
                list = list.Where(o => o.Status == status.Value).ToList();

            if (orderType.HasValue)
                list = list.Where(o => o.OrderType == orderType.Value.ToString()).ToList();

            return Ok(list);
        }

        /// <summary>Lấy đơn hàng của tôi - CUSTOMER</summary>
        [Authorize(Roles = "CUSTOMER")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMy()
        {
            var customer = await _customerService.GetByUserIdAsync(User.GetUserId());
            if (customer == null) return NotFound(new { message = "Không tìm thấy thông tin khách hàng" });
            return Ok(await _orderService.GetByCustomerIdAsync(customer.Id));
        }

        /// <summary>Lấy đơn hàng theo ID</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _orderService.GetByIdAsync(id);
            if (result == null) return NotFound(new { message = "Không tìm thấy đơn hàng" });

            // Customer chỉ xem đơn của mình
            var role = User.GetRole();
            if (role == "CUSTOMER")
            {
                var customer = await _customerService.GetByUserIdAsync(User.GetUserId());
                if (customer == null || result.CustomerId != customer.Id) return Forbid();
            }

            return Ok(result);
        }

        /// <summary>Tạo đơn hàng - CUSTOMER (online) hoặc EMPLOYEE (tại quầy)</summary>
        [Authorize(Roles = "CUSTOMER,EMPLOYEE,ADMIN")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            int customerId;
            var role = User.GetRole();

            if (role == "CUSTOMER")
            {
                var customer = await _customerService.GetByUserIdAsync(User.GetUserId());
                if (customer == null) return NotFound(new { message = "Không tìm thấy thông tin khách hàng" });
                customerId = customer.Id;
            }
            else
            {
                // EMPLOYEE/ADMIN tạo đơn cho khách tại quầy phải truyền CustomerId trong DTO
                if (!dto.CustomerId.HasValue)
                    return BadRequest(new { message = "Cần cung cấp CustomerId khi tạo đơn tại quầy" });
                customerId = dto.CustomerId.Value;
            }

            try
            {
                var result = await _orderService.CreateAsync(customerId, dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Cập nhật trạng thái đơn hàng - ADMIN, EMPLOYEE</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest req)
        {
            try
            {
                var result = await _orderService.UpdateStatusAsync(id, req.Status);
                return result == null ? NotFound() : Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Hủy đơn hàng</summary>
        [HttpDelete("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var result = await _orderService.CancelAsync(id, User.GetUserId(), User.GetRole());
                return result
                    ? Ok(new { message = "Đã hủy đơn hàng" })
                    : NotFound(new { message = "Không tìm thấy đơn hàng" });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public record UpdateOrderStatusRequest(OrderStatus Status);
}
