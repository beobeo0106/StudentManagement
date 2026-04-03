using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // <-- Bắt buộc phải có thư viện này
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Data;
using EcommerceAPI.Models;

namespace EcommerceAPI.Controllers
{
    [Route("api/v1/[controller]")] // Route sẽ là: /api/v1/categories
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/v1/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        // GET: api/v1/categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound(new { message = "Không tìm thấy danh mục!" });
            }

            return category;
        }

        // POST: api/v1/categories
        [HttpPost]
        [Authorize(Roles = "Admin")] // <-- CHỈ ADMIN
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        // PUT: api/v1/categories/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // <-- CHỈ ADMIN
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest(new { message = "ID danh mục không khớp!" });
            }

            category.UpdatedAt = DateTime.UtcNow;
            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound(new { message = "Không tìm thấy danh mục!" });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/v1/categories/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // <-- CHỈ ADMIN
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { message = "Không tìm thấy danh mục!" });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa danh mục thành công!" });
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}