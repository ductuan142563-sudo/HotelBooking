using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class ActivityLog
    {
        [Key]
        public int ActivityLogId { get; set; }

        public string? UserId { get; set; }

        [StringLength(100)]
        public string? UserName { get; set; }

        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty; // Login, Create, Update, Delete...

        [StringLength(100)]
        public string? Controller { get; set; }

        [StringLength(100)]
        public string? ActionName { get; set; }

        [StringLength(50)]
        public string? IpAddress { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
    }
}