using HotelBooking.Models;
using HotelBooking.ViewModels;
using HotelBooking.ViewModels.Admin;

namespace HotelBooking.Services
{
    public interface IHotelService
    {
        Task<HotelSearchViewModel> SearchAsync(HotelSearchViewModel search);
        Task<HotelDetailsViewModel?> GetDetailsAsync(int id);
        Task<List<Hotel>> GetAllAsync();
        Task<Hotel?> GetByIdAsync(int id);
        Task<(bool Success, string Message)> CreateAsync(HotelFormViewModel model, string webRootPath);
        Task<(bool Success, string Message)> UpdateAsync(HotelFormViewModel model, string webRootPath);
        Task<(bool Success, string Message)> DeleteAsync(int id);
        Task<(bool Success, string Message)> ChangeStatusAsync(int id, string status);
    }
}