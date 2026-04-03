using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; } // Giỏ hàng này của ai?
        public User? User { get; set; }

        [Required]
        public int ProductId { get; set; } // Món hàng gì?
        public Product? Product { get; set; }

        [Range(1, 1000)]
        public int Quantity { get; set; } // Số lượng bao nhiêu?

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}