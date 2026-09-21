using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class Hotel
    {
        [Key]
        public int HotelId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Range(1, 5)]
        public int Star { get; set; } = 3;

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? Website { get; set; }

        public TimeSpan CheckInTime { get; set; } = new TimeSpan(14, 0, 0);
        public TimeSpan CheckOutTime { get; set; } = new TimeSpan(12, 0, 0);

        public bool Status { get; set; } = true; // true = Active

        [StringLength(255)]
        public string? CoverImage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    }
}