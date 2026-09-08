using MassageShop.API.Helpers;
using MassageShop.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MassageShop.API.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _service;

        public InvoicesController(IInvoiceService service) => _service = service;

        /// <summary>Lấy tất cả hóa đơn - ADMIN, EMPLOYEE</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        /// <summary>Lấy hóa đơn theo ID</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound(new { message = "Không tìm thấy hóa đơn" }) : Ok(result);
        }

        /// <summary>Lấy hóa đơn theo đơn hàng</summary>
        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetByOrder(int orderId)
        {
            var result = await _service.GetByOrderIdAsync(orderId);
            return result == null ? NotFound(new { message = "Đơn hàng chưa có hóa đơn" }) : Ok(result);
        }

        /// <summary>Khách hàng xem hóa đơn đơn hàng của mình - CUSTOMER</summary>
        [Authorize(Roles = "CUSTOMER")]
        [HttpGet("my/order/{orderId}")]
        public async Task<IActionResult> GetMyInvoice(int orderId,
            [FromServices] ICustomerService customerService,
            [FromServices] IOrderService orderService)
        {
            var customer = await customerService.GetByUserIdAsync(User.GetUserId());
            if (customer == null) return NotFound(new { message = "Không tìm thấy thông tin khách hàng" });

            var order = await orderService.GetByIdAsync(orderId);
            if (order == null) return NotFound(new { message = "Không tìm thấy đơn hàng" });
            if (order.CustomerId != customer.Id) return Forbid();

            var result = await _service.GetByOrderIdAsync(orderId);
            return result == null ? NotFound(new { message = "Đơn hàng chưa có hóa đơn" }) : Ok(result);
        }

        /// <summary>Tạo hóa đơn cho đơn hàng - ADMIN, EMPLOYEE</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpPost("generate/{orderId}")]
        public async Task<IActionResult> Generate(int orderId)
        {
            try
            {
                var result = await _service.GenerateAsync(orderId);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
