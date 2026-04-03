using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using BCrypt.Net;

namespace EcommerceAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration; // Để đọc Secret Key từ appsettings.json
        }

        // POST: api/v1/auth/register
        [HttpPost("register")]
        public async Task<ActionResult> Register(UserRegisterDto request)
        {
            // 1. Kiểm tra xem Email đã tồn tại chưa
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { message = "Email này đã được sử dụng!" });
            }

            // 2. Mã hóa mật khẩu (Tuyệt đối không lưu mật khẩu trần)
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 3. Tạo User mới
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = request.Role,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đăng ký tài khoản thành công!" });
        }

        // POST: api/v1/auth/login
        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginDto request)
        {
            // 1. Tìm User theo Email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Email hoặc mật khẩu không đúng!" });
            }

            // 2. Kiểm tra mật khẩu
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest(new { message = "Email hoặc mật khẩu không đúng!" });
            }

            // 3. Tạo Token (Nếu đăng nhập đúng)
            string token = CreateToken(user);

            return Ok(new
            {
                message = "Đăng nhập thành công!",
                token = token
            });
        }

        // Hàm hỗ trợ tạo thẻ JWT
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role) // Lưu quyền (Admin/Customer) vào thẻ
            };

            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1), // Thẻ có hạn 1 ngày
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}