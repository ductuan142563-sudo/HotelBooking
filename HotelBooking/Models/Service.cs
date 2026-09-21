using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [StringLength(50)]
        public string Unit { get; set; } = "lần"; // lần, người, đêm...

        [StringLength(255)]
        public string? Image { get; set; }

        public bool Status { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
    }
}