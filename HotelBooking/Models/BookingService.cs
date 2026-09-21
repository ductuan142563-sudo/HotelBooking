using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class BookingService
    {
        [Key]
        public int BookingServiceId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int Quantity { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }

        // Navigation
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; } = null!;

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; } = null!;
    }
}