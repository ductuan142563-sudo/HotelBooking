namespace HotelBooking.ViewModels
{
    public class HotelListViewModel
    {
        public int HotelId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int Star { get; set; }
        public string? Thumbnail { get; set; }
        public string Status { get; set; } = "Active";
        public decimal? MinPrice { get; set; }          // Giá phòng rẻ nhất
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class HotelSearchViewModel
    {
        public string? Keyword { get; set; }
        public string? City { get; set; }
        public int? Star { get; set; }
        public string? SortBy { get; set; }             // price_asc, price_desc, rating, popular
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 9;

        public List<HotelListViewModel> Hotels { get; set; } = new();
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}