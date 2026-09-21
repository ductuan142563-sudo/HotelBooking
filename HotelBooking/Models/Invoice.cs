using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        [StringLength(30)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? CustomerEmail { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RoomAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ServiceAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; } = null!;
    }
}