using HotelBooking.Models;
using HotelBooking.ViewModels;

namespace HotelBooking.Services
{
    public interface IRoomTypeService
    {
        Task<IEnumerable<RoomType>> GetAllAsync();
        Task<RoomType?> GetByIdAsync(int id);
        Task<(bool Success, string Message)> CreateAsync(RoomTypeViewModel model);
        Task<(bool Success, string Message)> UpdateAsync(RoomTypeViewModel model);
        Task<(bool Success, string Message)> DeleteAsync(int id);
    }
}