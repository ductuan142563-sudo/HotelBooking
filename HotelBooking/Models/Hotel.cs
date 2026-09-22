using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class Hotel
    {
        [Key]
        public int HotelId { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(300)]
        public string Address { get; set; } = string.Empty;

        [Required, StringLength(100)]
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

        public TimeSpan CheckInTime { get; set; } = new TimeSpan(14, 0, 0);   // 14:00
        public TimeSpan CheckOutTime { get; set; } = new TimeSpan(12, 0, 0);  // 12:00

        [StringLength(20)]
        public string Status { get; set; } = "Active";   // Active, Inactive, Maintenance

        [StringLength(500)]
        public string? Thumbnail { get; set; }           // Ảnh đại diện

        [StringLength(2000)]
        public string? Gallery { get; set; }             // Nhiều ảnh, phân cách bằng |

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}