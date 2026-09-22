using HotelBooking.Data;
using HotelBooking.Models;
using HotelBooking.ViewModels;
using HotelBooking.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Services
{
    public class HotelService : IHotelService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HotelService> _logger;

        public HotelService(ApplicationDbContext context, ILogger<HotelService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<HotelSearchViewModel> SearchAsync(HotelSearchViewModel search)
        {
            var query = _context.Hotels
                .Include(h => h.Rooms)
                .Include(h => h.Reviews)
                .AsQueryable();

            // Chỉ hiện khách sạn Active cho phía Public
            query = query.Where(h => h.Status == "Active");

            if (!string.IsNullOrWhiteSpace(search.Keyword))
            {
                var kw = search.Keyword.Trim().ToLower();
                query = query.Where(h =>
                    h.Name.ToLower().Contains(kw) ||
                    h.Address.ToLower().Contains(kw) ||
                    h.City.ToLower().Contains(kw));
            }

            if (!string.IsNullOrWhiteSpace(search.City))
            {
                query = query.Where(h => h.City == search.City);
            }

            if (search.Star.HasValue)
            {
                query = query.Where(h => h.Star == search.Star.Value);
            }

            // Sắp xếp
            query = search.SortBy switch
            {
                "price_asc" => query.OrderBy(h => h.Rooms.Min(r => (decimal?)r.Price) ?? decimal.MaxValue),
                "price_desc" => query.OrderByDescending(h => h.Rooms.Min(r => (decimal?)r.Price) ?? 0),
                "rating" => query.OrderByDescending(h => h.Reviews.Average(r => (double?)r.Rating) ?? 0),
                _ => query.OrderByDescending(h => h.CreatedAt)
            };

            search.TotalItems = await query.CountAsync();

            var hotels = await query
                .Skip((search.Page - 1) * search.PageSize)
                .Take(search.PageSize)
                .Select(h => new HotelListViewModel
                {
                    HotelId = h.HotelId,
                    Name = h.Name,
                    Address = h.Address,
                    City = h.City,
                    Star = h.Star,
                    Thumbnail = h.Thumbnail,
                    Status = h.Status,
                    MinPrice = h.Rooms.Any() ? h.Rooms.Min(r => r.Price) : null,
                    AverageRating = h.Reviews.Any() ? h.Reviews.Average(r => r.Rating) : 0,
                    ReviewCount = h.Reviews.Count
                })
                .ToListAsync();

            search.Hotels = hotels;
            return search;
        }

        public async Task<HotelDetailsViewModel?> GetDetailsAsync(int id)
        {
            var hotel = await _context.Hotels
                .Include(h => h.Rooms)
                    .ThenInclude(r => r.RoomType)
                .Include(h => h.Reviews)
                .FirstOrDefaultAsync(h => h.HotelId == id && h.Status == "Active");

            if (hotel == null)
                return null;

            var vm = new HotelDetailsViewModel
            {
                HotelId = hotel.HotelId,
                Name = hotel.Name,
                Address = hotel.Address,
                City = hotel.City,
                Description = hotel.Description,
                Star = hotel.Star,
                Phone = hotel.Phone,
                Email = hotel.Email,
                Website = hotel.Website,
                CheckInTime = hotel.CheckInTime,
                CheckOutTime = hotel.CheckOutTime,
                Status = hotel.Status,
                Thumbnail = hotel.Thumbnail,
                AverageRating = hotel.Reviews.Any() ? hotel.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = hotel.Reviews.Count,
                GalleryImages = string.IsNullOrEmpty(hotel.Gallery)
                    ? new List<string>()
                    : hotel.Gallery.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList()
            };

            // Danh sách phòng (sẽ hoàn thiện hơn ở PART 6)
            vm.AvailableRooms = hotel.Rooms
                .Where(r => r.Status == "Available")
                .Select(r => new RoomCardViewModel
                {
                    RoomId = r.RoomId,
                    RoomNumber = r.RoomNumber,
                    RoomTypeName = r.RoomType?.Name ?? "N/A",
                    Price = r.Price,
                    Capacity = r.Capacity,
                    Image = r.RoomType?.Image,
                    Status = r.Status
                })
                .ToList();

            return vm;
        }

        public async Task<List<Hotel>> GetAllAsync()
        {
            return await _context.Hotels
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }

        public async Task<Hotel?> GetByIdAsync(int id)
        {
            return await _context.Hotels.FindAsync(id);
        }

        public async Task<(bool Success, string Message)> CreateAsync(HotelFormViewModel model, string webRootPath)
        {
            try
            {
                var hotel = new Hotel
                {
                    Name = model.Name,
                    Address = model.Address,
                    City = model.City,
                    Description = model.Description,
                    Star = model.Star,
                    Phone = model.Phone,
                    Email = model.Email,
                    Website = model.Website,
                    CheckInTime = model.CheckInTime,
                    CheckOutTime = model.CheckOutTime,
                    Status = model.Status,
                    CreatedAt = DateTime.UtcNow
                };

                // Upload Thumbnail
                if (model.ThumbnailFile != null && model.ThumbnailFile.Length > 0)
                {
                    hotel.Thumbnail = await SaveImageAsync(model.ThumbnailFile, webRootPath, "hotels");
                }

                // Upload Gallery
                if (model.GalleryFiles != null && model.GalleryFiles.Any())
                {
                    var paths = new List<string>();
                    foreach (var file in model.GalleryFiles)
                    {
                        if (file.Length > 0)
                        {
                            paths.Add(await SaveImageAsync(file, webRootPath, "hotels/gallery"));
                        }
                    }
                    hotel.Gallery = string.Join("|", paths);
                }

                _context.Hotels.Add(hotel);
                await _context.SaveChangesAsync();

                return (true, "Thêm khách sạn thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tạo Hotel");
                return (false, "Có lỗi xảy ra khi tạo khách sạn.");
            }
        }

        public async Task<(bool Success, string Message)> UpdateAsync(HotelFormViewModel model, string webRootPath)
        {
            try
            {
                var hotel = await _context.Hotels.FindAsync(model.HotelId);
                if (hotel == null)
                    return (false, "Không tìm thấy khách sạn.");

                hotel.Name = model.Name;
                hotel.Address = model.Address;
                hotel.City = model.City;
                hotel.Description = model.Description;
                hotel.Star = model.Star;
                hotel.Phone = model.Phone;
                hotel.Email = model.Email;
                hotel.Website = model.Website;
                hotel.CheckInTime = model.CheckInTime;
                hotel.CheckOutTime = model.CheckOutTime;
                hotel.Status = model.Status;
                hotel.UpdatedAt = DateTime.UtcNow;

                // Cập nhật Thumbnail nếu có file mới
                if (model.ThumbnailFile != null && model.ThumbnailFile.Length > 0)
                {
                    hotel.Thumbnail = await SaveImageAsync(model.ThumbnailFile, webRootPath, "hotels");
                }

                // Cập nhật Gallery nếu có file mới
                if (model.GalleryFiles != null && model.GalleryFiles.Any())
                {
                    var paths = new List<string>();
                    foreach (var file in model.GalleryFiles)
                    {
                        if (file.Length > 0)
                        {
                            paths.Add(await SaveImageAsync(file, webRootPath, "hotels/gallery"));
                        }
                    }
                    hotel.Gallery = string.Join("|", paths);
                }

                await _context.SaveChangesAsync();
                return (true, "Cập nhật khách sạn thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật Hotel");
                return (false, "Có lỗi xảy ra khi cập nhật.");
            }
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            try
            {
                var hotel = await _context.Hotels
                    .Include(h => h.Rooms)
                    .Include(h => h.Bookings)
                    .FirstOrDefaultAsync(h => h.HotelId == id);

                if (hotel == null)
                    return (false, "Không tìm thấy khách sạn.");

                if (hotel.Bookings.Any())
                    return (false, "Không thể xóa khách sạn đã có booking.");

                _context.Hotels.Remove(hotel);
                await _context.SaveChangesAsync();

                return (true, "Xóa khách sạn thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xóa Hotel");
                return (false, "Có lỗi xảy ra khi xóa.");
            }
        }

        public async Task<(bool Success, string Message)> ChangeStatusAsync(int id, string status)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
                return (false, "Không tìm thấy khách sạn.");

            hotel.Status = status;
            hotel.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return (true, "Đã cập nhật trạng thái.");
        }

        // ===================== Helper =====================
        private async Task<string> SaveImageAsync(IFormFile file, string webRootPath, string folder)
        {
            var uploadsFolder = Path.Combine(webRootPath, "uploads", folder);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Trả về đường dẫn tương đối để lưu vào database
            return $"/uploads/{folder}/{uniqueFileName}";
        }
    }
}