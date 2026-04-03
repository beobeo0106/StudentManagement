using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Slug { get; set; } = string.Empty;

        // Cho phép danh mục cha (để làm menu đa cấp như: Điện thoại -> iPhone)
        public int? ParentId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Quan hệ: Một danh mục có thể có nhiều sản phẩm
        public ICollection<Product>? Products { get; set; }
    }
}