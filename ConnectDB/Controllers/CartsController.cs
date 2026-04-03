using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using System.Security.Claims;

namespace EcommerceAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize] // Phải đăng nhập mới có giỏ hàng cá nhân
    public class CartsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartsController(AppDbContext context)
        {
            _context = context;
        }

        // 1. Lấy danh sách giỏ hàng của tôi
        [HttpGet]
        public async Task<IActionResult> GetMyCart()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product) // Lấy kèm thông tin tên, giá, ảnh sản phẩm
                .ToListAsync();

            return Ok(cartItems);
        }

        // 2. Thêm sản phẩm vào giỏ
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            // Kiểm tra xem món này đã có trong giỏ chưa
            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (existingItem != null)
            {
                // Nếu có rồi thì cộng dồn số lượng
                existingItem.Quantity += quantity;
            }
            else
            {
                // Nếu chưa có thì tạo mới
                var newItem = new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                };
                _context.CartItems.Add(newItem);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã thêm vào giỏ hàng!" });
        }

        // 3. Xóa một món khỏi giỏ
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var item = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (item == null) return NotFound();

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã xóa khỏi giỏ hàng!" });
        }
    }
}