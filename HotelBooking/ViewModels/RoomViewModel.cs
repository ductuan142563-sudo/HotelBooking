using System.ComponentModel.DataAnnotations;
using HotelBooking.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelBooking.ViewModels
{
    public class RoomViewModel
    {
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn khách sạn")]
        [Display(Name = "Khách sạn")]
        public int HotelId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại phòng")]
        [Display(Name = "Loại phòng")]
        public int RoomTypeId { get; set; }

        [Required(ErrorMessage = "Số phòng là bắt buộc")]
        [StringLength(20)]
        [Display(Name = "Số phòng")]
        public string RoomNumber { get; set; } = string.Empty;

        [Required]
        [Range(0, 100000000)]
        [Display(Name = "Giá / đêm")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 20)]
        [Display(Name = "Sức chứa")]
        public int Capacity { get; set; }

        [Display(Name = "Tầng")]
        public int? Floor { get; set; }

        [StringLength(1000)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public RoomStatus Status { get; set; } = RoomStatus.Available;

        [Display(Name = "Ảnh hiện tại")]
        public string? ExistingImage { get; set; }

        [Display(Name = "Upload ảnh mới")]
        public IFormFile? ImageFile { get; set; }

        // Dropdown lists
        public SelectList? Hotels { get; set; }
        public SelectList? RoomTypes { get; set; }
    }

    public class RoomListViewModel
    {
        public IEnumerable<Room> Rooms { get; set; } = new List<Room>();
        public string? SearchTerm { get; set; }
        public int? HotelId { get; set; }
        public int? RoomTypeId { get; set; }
        public RoomStatus? Status { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public SelectList? Hotels { get; set; }
        public SelectList? RoomTypes { get; set; }
    }
}