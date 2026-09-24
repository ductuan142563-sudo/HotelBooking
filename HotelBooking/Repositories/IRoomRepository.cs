using HotelBooking.Models;

namespace HotelBooking.Repositories
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetAllAsync();
        Task<Room?> GetByIdAsync(int id);
        Task<IEnumerable<Room>> GetByHotelIdAsync(int hotelId);
        Task AddAsync(Room room);
        Task UpdateAsync(Room room);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> RoomNumberExistsAsync(int hotelId, string roomNumber, int? excludeId = null);
        Task<(IEnumerable<Room> Rooms, int TotalCount)> SearchAsync(
            string? searchTerm, int? hotelId, int? roomTypeId, RoomStatus? status,
            int page, int pageSize);
    }
}