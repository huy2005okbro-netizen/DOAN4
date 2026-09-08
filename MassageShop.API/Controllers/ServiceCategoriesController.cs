using MassageShop.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MassageShop.API.Controllers
{
    [ApiController]
    [Route("api/service-categories")]
    public class ServiceCategoriesController : ControllerBase
    {
        private readonly IServiceCategoryService _service;

        public ServiceCategoriesController(IServiceCategoryService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest req)
        {
            var result = await _service.CreateAsync(req.Name, req.Description);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest req)
        {
            var result = await _service.UpdateAsync(id, req.Name, req.Description, req.IsActive);
            return result == null ? NotFound() : Ok(result);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result ? Ok(new { message = "Đã xóa danh mục" }) : NotFound();
        }
    }

    public record CreateCategoryRequest(string Name, string? Description);
    public record UpdateCategoryRequest(string? Name, string? Description, bool? IsActive);
}
