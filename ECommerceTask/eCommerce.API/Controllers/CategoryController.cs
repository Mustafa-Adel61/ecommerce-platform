using eCommerce.Application.DTOs.Category;
using eCommerce.Application.Interfaces.IRepository;
using eCommerce.Application.Interfaces.IService;
using eCommerce.Domain.Models;
using FitPro.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // POST: api/category
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto, CancellationToken ct)
        {
            try
            {
                var category = await _categoryService.CreateAsync(dto, ct);
                return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/category/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var category = await _categoryService.GetByIdAsync(id, ct);
            if (category == null) return NotFound();

            return Ok(category);
        }

        // GET: api/category
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var categories = await _categoryService.GetAllAsync(ct);
            return Ok(categories);
        }

        // PUT: api/category/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryCreateDto dto, CancellationToken ct)
        {
            var updatedCategory = await _categoryService.UpdateAsync(id, dto, ct);
            if (updatedCategory == null) return NotFound();

            return Ok(updatedCategory);
        }

        // DELETE: api/category/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _categoryService.DeleteAsync(id, ct);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
