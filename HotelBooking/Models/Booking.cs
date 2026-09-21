using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public enum BookingStatus
    {
        Pending = 0,
        Confirmed = 1,
        CheckedIn = 2,
        CheckedOut = 3,
        Cancelled = 4,
        Completed = 5
    }

    public enum PaymentStatus
    {
        Unpaid = 0,
        Paid = 1,
        Partial = 2,
        Refunded = 3,
        Failed = 4
    }

    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        [StringLength(20)]
        public string BookingCode { get; set; } = string.Empty; // Ví dụ: BK20260921001

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int HotelId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Range(1, 50)]
        public int NumberOfGuests { get; set; }

        [Range(1, 20)]
        public int NumberOfRooms { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalRoomPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalServicePrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalAmount { get; set; }

        public BookingStatus BookingStatus { get; set; } = BookingStatus.Pending;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

        public int? PromotionId { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        [ForeignKey("HotelId")]
        public virtual Hotel Hotel { get; set; } = null!;

        [ForeignKey("PromotionId")]
        public virtual Promotion? Promotion { get; set; }

        public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
        public virtual ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
        public virtual Payment? Payment { get; set; }
        public virtual Invoice? Invoice { get; set; }
    }
}