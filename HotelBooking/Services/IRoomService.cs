using HotelBooking.Models;
using HotelBooking.ViewModels;

namespace HotelBooking.Services
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetAllAsync();
        Task<Room?> GetByIdAsync(int id);
        Task<(IEnumerable<Room> Rooms, int TotalCount)> SearchAsync(
            string? searchTerm, int? hotelId, int? roomTypeId, RoomStatus? status, int page, int pageSize);
        Task<(bool Success, string Message)> CreateAsync(RoomViewModel model);
        Task<(bool Success, string Message)> UpdateAsync(RoomViewModel model);
        Task<(bool Success, string Message)> DeleteAsync(int id);
        Task<(bool Success, string Message)> ChangeStatusAsync(int id, RoomStatus status);
    }
}