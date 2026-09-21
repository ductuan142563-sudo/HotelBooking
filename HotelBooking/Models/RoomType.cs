using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class RoomType
    {
        [Key]
        public int RoomTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty; // Single, Double, Twin, Deluxe, Suite, Family

        [StringLength(1000)]
        public string? Description { get; set; }

        [Range(1, 20)]
        public int MaxGuests { get; set; } = 2;

        [StringLength(100)]
        public string? BedType { get; set; }

        public decimal Area { get; set; } // m²

        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }

        [StringLength(255)]
        public string? Image { get; set; }

        public bool Status { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}