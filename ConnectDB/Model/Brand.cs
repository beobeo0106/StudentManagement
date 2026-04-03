using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class Brand
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên thương hiệu không được để trống")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Slug { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Quan hệ 1-Nhiều: 1 Thương hiệu có nhiều Sản phẩm
        public ICollection<Product>? Products { get; set; }
    }
}