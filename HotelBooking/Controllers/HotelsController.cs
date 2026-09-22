using HotelBooking.Services;
using HotelBooking.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers
{
    public class HotelsController : Controller
    {
        private readonly IHotelService _hotelService;

        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        // GET: /Hotels
        public async Task<IActionResult> Index(HotelSearchViewModel search)
        {
            var result = await _hotelService.SearchAsync(search);
            return View(result);
        }

        // GET: /Hotels/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var hotel = await _hotelService.GetDetailsAsync(id);
            if (hotel == null)
                return NotFound();

            return View(hotel);
        }
    }
}