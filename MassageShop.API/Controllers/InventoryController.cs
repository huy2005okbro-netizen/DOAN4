using MassageShop.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MassageShop.API.Controllers
{
    [ApiController]
    [Route("api/inventory")]
    [Authorize(Roles = "ADMIN,EMPLOYEE")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _service;

        public InventoryController(IInventoryService service) => _service = service;

        /// <summary>Lấy tất cả giao dịch kho</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        /// <summary>Lấy giao dịch kho theo sản phẩm</summary>
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProduct(int productId) =>
            Ok(await _service.GetByProductIdAsync(productId));

        /// <summary>Nhập kho - ADMIN, EMPLOYEE</summary>
        [HttpPost("import")]
        public async Task<IActionResult> Import([FromBody] InventoryRequest req)
        {
            try
            {
                var result = await _service.ImportAsync(req.ProductId, req.Quantity, req.Note);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Điều chỉnh tồn kho - ADMIN</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpPost("adjust")]
        public async Task<IActionResult> Adjust([FromBody] InventoryRequest req)
        {
            try
            {
                var result = await _service.AdjustAsync(req.ProductId, req.Quantity, req.Note);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public record InventoryRequest(int ProductId, int Quantity, string? Note);
}
