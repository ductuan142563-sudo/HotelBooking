using HotelBooking.Data;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _context;

        public RoomRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            return await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .OrderBy(r => r.Hotel!.Name)
                .ThenBy(r => r.RoomNumber)
                .ToListAsync();
        }

        public async Task<Room?> GetByIdAsync(int id)
        {
            return await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.RoomId == id);
        }

        public async Task<IEnumerable<Room>> GetByHotelIdAsync(int hotelId)
        {
            return await _context.Rooms
                .Include(r => r.RoomType)
                .Where(r => r.HotelId == hotelId)
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();
        }

        public async Task AddAsync(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Room room)
        {
            room.UpdatedAt = DateTime.Now;
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Rooms.AnyAsync(r => r.RoomId == id);
        }

        public async Task<bool> RoomNumberExistsAsync(int hotelId, string roomNumber, int? excludeId = null)
        {
            return await _context.Rooms
                .AnyAsync(r => r.HotelId == hotelId
                    && r.RoomNumber == roomNumber
                    && (!excludeId.HasValue || r.RoomId != excludeId.Value));
        }

        public async Task<(IEnumerable<Room> Rooms, int TotalCount)> SearchAsync(
            string? searchTerm, int? hotelId, int? roomTypeId, RoomStatus? status,
            int page, int pageSize)
        {
            var query = _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(r => r.RoomNumber.Contains(searchTerm)
                    || (r.Description != null && r.Description.Contains(searchTerm)));
            }

            if (hotelId.HasValue)
                query = query.Where(r => r.HotelId == hotelId.Value);

            if (roomTypeId.HasValue)
                query = query.Where(r => r.RoomTypeId == roomTypeId.Value);

            if (status.HasValue)
                query = query.Where(r => r.Status == status.Value);

            var totalCount = await query.CountAsync();

            var rooms = await query
                .OrderBy(r => r.Hotel!.Name)
                .ThenBy(r => r.RoomNumber)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (rooms, totalCount);
        }
    }
}