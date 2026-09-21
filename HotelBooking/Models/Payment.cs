using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public enum PaymentMethod
    {
        Cash = 0,
        BankTransfer = 1,
        VNPay = 2,
        MoMo = 3,
        CreditCard = 4
    }

    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

        [StringLength(100)]
        public string? TransactionCode { get; set; }

        public DateTime? PaidAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; } = null!;
    }
}