using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HotelBooking.ViewModels.Admin
{
    public class HotelFormViewModel
    {
        public int HotelId { get; set; }

        [Required(ErrorMessage = "Tên khách sạn không được để trống")]
        [StringLength(200)]
        [Display(Name = "Tên khách sạn")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [StringLength(300)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Thành phố không được để trống")]
        [StringLength(100)]
        [Display(Name = "Thành phố")]
        public string City { get; set; } = string.Empty;

        [StringLength(2000)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Range(1, 5, ErrorMessage = "Số sao từ 1 đến 5")]
        [Display(Name = "Số sao")]
        public int Star { get; set; } = 3;

        [StringLength(20)]
        [Display(Name = "Điện thoại")]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(200)]
        [Display(Name = "Website")]
        public string? Website { get; set; }

        [Display(Name = "Giờ nhận phòng")]
        public TimeSpan CheckInTime { get; set; } = new TimeSpan(14, 0, 0);

        [Display(Name = "Giờ trả phòng")]
        public TimeSpan CheckOutTime { get; set; } = new TimeSpan(12, 0, 0);

        [Required]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Active";

        [Display(Name = "Ảnh đại diện")]
        public IFormFile? ThumbnailFile { get; set; }

        public string? CurrentThumbnail { get; set; }

        [Display(Name = "Gallery (nhiều ảnh)")]
        public List<IFormFile>? GalleryFiles { get; set; }

        public string? CurrentGallery { get; set; }
    }
}