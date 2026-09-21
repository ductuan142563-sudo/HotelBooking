using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public enum DiscountType
    {
        Percentage = 0,
        FixedAmount = 1
    }

    public class Promotion
    {
        [Key]
        public int PromotionId { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public DiscountType DiscountType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountValue { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MinimumAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxDiscount { get; set; }

        public int UsageLimit { get; set; }
        public int UsedCount { get; set; } = 0;

        public bool Status { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}