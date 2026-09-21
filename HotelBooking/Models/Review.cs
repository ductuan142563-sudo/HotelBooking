using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int HotelId { get; set; }

        public int? RoomId { get; set; }

        public int? BookingId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(2000)]
        public string? Comment { get; set; }

        [StringLength(500)]
        public string? Images { get; set; } // JSON hoặc comma-separated

        public bool IsApproved { get; set; } = false;
        public bool IsHidden { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        [ForeignKey("HotelId")]
        public virtual Hotel Hotel { get; set; } = null!;

        [ForeignKey("RoomId")]
        public virtual Room? Room { get; set; }

        [ForeignKey("BookingId")]
        public virtual Booking? Booking { get; set; }
    }
}