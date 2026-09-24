using HotelBooking.Models;
using HotelBooking.Repositories;
using HotelBooking.ViewModels;

namespace HotelBooking.Services
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IRoomTypeRepository _repo;
        private readonly IWebHostEnvironment _env;

        public RoomTypeService(IRoomTypeRepository repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }

        public async Task<IEnumerable<RoomType>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<RoomType?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

        public async Task<(bool Success, string Message)> CreateAsync(RoomTypeViewModel model)
        {
            if (await _repo.NameExistsAsync(model.Name))
                return (false, "Tên loại phòng đã tồn tại.");

            var roomType = new RoomType
            {
                Name = model.Name,
                Description = model.Description,
                MaxGuests = model.MaxGuests,
                BedType = model.BedType,
                Area = model.Area,
                BasePrice = model.BasePrice,
                IsActive = model.IsActive,
                CreatedAt = DateTime.Now
            };

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                roomType.Image = await SaveImageAsync(model.ImageFile, "roomtypes");
            }

            await _repo.AddAsync(roomType);
            return (true, "Thêm loại phòng thành công.");
        }

        public async Task<(bool Success, string Message)> UpdateAsync(RoomTypeViewModel model)
        {
            var roomType = await _repo.GetByIdAsync(model.RoomTypeId);
            if (roomType == null)
                return (false, "Không tìm thấy loại phòng.");

            if (await _repo.NameExistsAsync(model.Name, model.RoomTypeId))
                return (false, "Tên loại phòng đã tồn tại.");

            roomType.Name = model.Name;
            roomType.Description = model.Description;
            roomType.MaxGuests = model.MaxGuests;
            roomType.BedType = model.BedType;
            roomType.Area = model.Area;
            roomType.BasePrice = model.BasePrice;
            roomType.IsActive = model.IsActive;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(roomType.Image))
                    DeleteImage(roomType.Image);

                roomType.Image = await SaveImageAsync(model.ImageFile, "roomtypes");
            }

            await _repo.UpdateAsync(roomType);
            return (true, "Cập nhật loại phòng thành công.");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var roomType = await _repo.GetByIdAsync(id);
            if (roomType == null)
                return (false, "Không tìm thấy loại phòng.");

            if (!string.IsNullOrEmpty(roomType.Image))
                DeleteImage(roomType.Image);

            await _repo.DeleteAsync(id);
            return (true, "Xóa loại phòng thành công.");
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