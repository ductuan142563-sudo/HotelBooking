using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HotelBooking.ViewModels
{
    public class RoomTypeViewModel
    {
        public int RoomTypeId { get; set; }

        [Required(ErrorMessage = "Tên loại phòng là bắt buộc")]
        [StringLength(100)]
        [Display(Name = "Tên loại phòng")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Required]
        [Range(1, 20, ErrorMessage = "Số khách từ 1-20")]
        [Display(Name = "Số khách tối đa")]
        public int MaxGuests { get; set; }

        [StringLength(50)]
        [Display(Name = "Loại giường")]
        public string? BedType { get; set; }

        [Display(Name = "Diện tích (m²)")]
        public decimal? Area { get; set; }

        [Required]
        [Range(0, 100000000)]
        [Display(Name = "Giá cơ bản")]
        public decimal BasePrice { get; set; }

        [Display(Name = "Ảnh hiện tại")]
        public string? ExistingImage { get; set; }

        [Display(Name = "Upload ảnh mới")]
        public IFormFile? ImageFile { get; set; }

        public bool IsActive { get; set; } = true;
    }
}