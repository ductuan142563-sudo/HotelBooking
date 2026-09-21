using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class Wishlist
    {
        [Key]
        public int WishlistId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int HotelId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        [ForeignKey("HotelId")]
        public virtual Hotel Hotel { get; set; } = null!;
    }
}