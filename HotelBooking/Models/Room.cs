using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public enum RoomStatus
    {
        Available = 0,
        Occupied = 1,
        Maintenance = 2,
        Inactive = 3
    }

    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [Required]
        public int HotelId { get; set; }

        [Required]
        public int RoomTypeId { get; set; }

        [Required]
        [StringLength(20)]
        public string RoomNumber { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Range(1, 20)]
        public int Capacity { get; set; } = 2;

        public int Floor { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public RoomStatus Status { get; set; } = RoomStatus.Available;

        [StringLength(255)]
        public string? Image { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        [ForeignKey("HotelId")]
        public virtual Hotel Hotel { get; set; } = null!;

        [ForeignKey("RoomTypeId")]
        public virtual RoomType RoomType { get; set; } = null!;

        public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
    }
}