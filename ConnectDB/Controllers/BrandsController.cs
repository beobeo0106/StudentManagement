using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // <-- Bắt buộc phải có thư viện này
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Data;
using EcommerceAPI.Models;

namespace EcommerceAPI.Controllers
{
    [Route("api/v1/[controller]")] // Route: /api/v1/brands
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BrandsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> GetBrands()
        {
            return await _context.Brands.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Brand>> GetBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null) return NotFound(new { message = "Không tìm thấy thương hiệu!" });
            return brand;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // <-- CHỈ ADMIN
        public async Task<ActionResult<Brand>> PostBrand(Brand brand)
        {
            brand.CreatedAt = DateTime.UtcNow;
            brand.UpdatedAt = DateTime.UtcNow;

            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBrand), new { id = brand.Id }, brand);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // <-- CHỈ ADMIN
        public async Task<IActionResult> PutBrand(int id, Brand brand)
        {
            if (id != brand.Id) return BadRequest(new { message = "ID không khớp!" });

            brand.UpdatedAt = DateTime.UtcNow;
            _context.Entry(brand).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrandExists(id)) return NotFound(new { message = "Không tìm thấy thương hiệu!" });
                else throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // <-- CHỈ ADMIN
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null) return NotFound(new { message = "Không tìm thấy thương hiệu!" });

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa thương hiệu thành công!" });
        }

        private bool BrandExists(int id)
        {
            return _context.Brands.Any(e => e.Id == id);
        }
    }
}