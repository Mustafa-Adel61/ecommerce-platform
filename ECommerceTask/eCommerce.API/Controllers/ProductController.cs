using eCommerce.Application.DTOs.Product;
using eCommerce.Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _env;

        public ProductController(IProductService productService, IWebHostEnvironment env)
        {
            _productService = productService;
            _env = env;
        }

        // GET: api/products?pageNumber=2&pageSize=5
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _productService.GetAllAsync(pageNumber, pageSize, ct);
            return Ok(result);
        }

        // GET: api/product/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var product = await _productService.GetByIdAsync(id, ct);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // GET: api/product/by-code/{code}
        [HttpGet("by-code/{code}")]
        public async Task<IActionResult> GetByCode(string code, CancellationToken ct)
        {
            var product = await _productService.GetByCodeAsync(code, ct);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // GET: api/product/by-category/{categoryId}
        [HttpGet("by-category/{categoryId:int}")]
        public async Task<IActionResult> GetByCategory(int categoryId, CancellationToken ct)
        {
            var products = await _productService.GetByCategoryAsync(categoryId, ct);
            return Ok(products);
        }

        // POST: api/product
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateDto dto, CancellationToken ct)
        {
            try
            {
                var product = await _productService.CreateAsync(dto, _env.WebRootPath);
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/product/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromForm] ProductUpdateDto dto, CancellationToken ct)
        {
            var updated = await _productService.UpdateAsync(id, dto, _env.WebRootPath);
            if (!updated) return NotFound();
            return NoContent();
        }

        // DELETE: api/product/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _productService.DeleteAsync(id, ct);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
