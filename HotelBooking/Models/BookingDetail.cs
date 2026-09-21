using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class BookingDetail
    {
        [Key]
        public int BookingDetailId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } // Giá tại thời điểm đặt

        public int Quantity { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }

        // Navigation
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; } = null!;

        [ForeignKey("RoomId")]
        public virtual Room Room { get; set; } = null!;
    }
}