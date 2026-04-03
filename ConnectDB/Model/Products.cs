using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Sku { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string Slug { get; set; } = string.Empty;

        public string? ShortDesc { get; set; }
        public string? LongDesc { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Price { get; set; }

        public int Stock { get; set; } = 0;

        // Foreign Keys (Nullable)
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }

        // Navigation properties (Quan hệ với các bảng khác)
        [ForeignKey("BrandId")]
        public Brand? Brand { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        // 1 Sản phẩm có nhiều Ảnh
        public ICollection<ProductImage>? Images { get; set; }
    }
}