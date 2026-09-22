namespace HotelBooking.ViewModels
{
    public class HotelDetailsViewModel
    {
        public int HotelId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Star { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public TimeSpan CheckInTime { get; set; }
        public TimeSpan CheckOutTime { get; set; }
        public string Status { get; set; } = "Active";
        public string? Thumbnail { get; set; }
        public List<string> GalleryImages { get; set; } = new();
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

        // Sẽ mở rộng ở PART sau (Rooms, Services, Reviews)
        public List<RoomCardViewModel> AvailableRooms { get; set; } = new();
    }

    public class RoomCardViewModel
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomTypeName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public string? Image { get; set; }
        public string Status { get; set; } = "Available";
    }
}