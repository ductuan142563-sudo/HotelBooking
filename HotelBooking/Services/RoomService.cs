using HotelBooking.Models;
using HotelBooking.Repositories;
using HotelBooking.ViewModels;

namespace HotelBooking.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _repo;
        private readonly IWebHostEnvironment _env;

        public RoomService(IRoomRepository repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }

        public async Task<IEnumerable<Room>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<Room?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

        public async Task<(IEnumerable<Room> Rooms, int TotalCount)> SearchAsync(
            string? searchTerm, int? hotelId, int? roomTypeId, RoomStatus? status, int page, int pageSize)
        {
            return await _repo.SearchAsync(searchTerm, hotelId, roomTypeId, status, page, pageSize);
        }

        public async Task<(bool Success, string Message)> CreateAsync(RoomViewModel model)
        {
            if (await _repo.RoomNumberExistsAsync(model.HotelId, model.RoomNumber))
                return (false, "Số phòng đã tồn tại trong khách sạn này.");

            var room = new Room
            {
                HotelId = model.HotelId,
                RoomTypeId = model.RoomTypeId,
                RoomNumber = model.RoomNumber,
                Price = model.Price,
                Capacity = model.Capacity,
                Floor = model.Floor,
                Description = model.Description,
                Status = model.Status,
                CreatedAt = DateTime.Now
            };

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                room.Image = await SaveImageAsync(model.ImageFile, "rooms");
            }

            await _repo.AddAsync(room);
            return (true, "Thêm phòng thành công.");
        }

        public async Task<(bool Success, string Message)> UpdateAsync(RoomViewModel model)
        {
            var room = await _repo.GetByIdAsync(model.RoomId);
            if (room == null)
                return (false, "Không tìm thấy phòng.");

            if (await _repo.RoomNumberExistsAsync(model.HotelId, model.RoomNumber, model.RoomId))
                return (false, "Số phòng đã tồn tại trong khách sạn này.");

            room.HotelId = model.HotelId;
            room.RoomTypeId = model.RoomTypeId;
            room.RoomNumber = model.RoomNumber;
            room.Price = model.Price;
            room.Capacity = model.Capacity;
            room.Floor = model.Floor;
            room.Description = model.Description;
            room.Status = model.Status;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(room.Image))
                    DeleteImage(room.Image);

                room.Image = await SaveImageAsync(model.ImageFile, "rooms");
            }

            await _repo.UpdateAsync(room);
            return (true, "Cập nhật phòng thành công.");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var room = await _repo.GetByIdAsync(id);
            if (room == null)
                return (false, "Không tìm thấy phòng.");

            if (!string.IsNullOrEmpty(room.Image))
                DeleteImage(room.Image);

            await _repo.DeleteAsync(id);
            return (true, "Xóa phòng thành công.");
        }

        public async Task<(bool Success, string Message)> ChangeStatusAsync(int id, RoomStatus status)
        {
            var room = await _repo.GetByIdAsync(id);
            if (room == null)
                return (false, "Không tìm thấy phòng.");

            room.Status = status;
            await _repo.UpdateAsync(room);
            return (true, $"Đã đổi trạng thái phòng thành {status}.");
        }

        private async Task<string> SaveImageAsync(IFormFile file, string folder)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", folder);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{folder}/{uniqueName}";
        }

        private void DeleteImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;
            var fullPath = Path.Combine(_env.WebRootPath, imagePath.TrimStart('/'));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}