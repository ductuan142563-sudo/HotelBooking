using HotelBooking.Services;
using HotelBooking.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HotelsController : Controller
    {
        private readonly IHotelService _hotelService;
        private readonly IWebHostEnvironment _env;

        public HotelsController(IHotelService hotelService, IWebHostEnvironment env)
        {
            _hotelService = hotelService;
            _env = env;
        }

        public async Task<IActionResult> Index(string? keyword, string? status, int page = 1)
        {
            var hotels = await _hotelService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim().ToLower();
                hotels = hotels.Where(h =>
                    h.Name.ToLower().Contains(keyword) ||
                    h.City.ToLower().Contains(keyword)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(status))
                hotels = hotels.Where(h => h.Status == status).ToList();

            ViewBag.Keyword = keyword;
            ViewBag.Status = status;
            return View(hotels);
        }

        public IActionResult Create()
        {
            return View(new HotelFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HotelFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (success, message) = await _hotelService.CreateAsync(model, _env.WebRootPath);

            if (success)
            {
                TempData["Success"] = message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, message);
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var hotel = await _hotelService.GetByIdAsync(id);
            if (hotel == null) return NotFound();

            var model = new HotelFormViewModel
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
                CurrentThumbnail = hotel.Thumbnail,
                CurrentGallery = hotel.Gallery
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HotelFormViewModel model)
        {
            if (id != model.HotelId) return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var (success, message) = await _hotelService.UpdateAsync(model, _env.WebRootPath);

            if (success)
            {
                TempData["Success"] = message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, message);
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var hotel = await _hotelService.GetByIdAsync(id);
            if (hotel == null) return NotFound();
            return View(hotel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var hotel = await _hotelService.GetByIdAsync(id);
            if (hotel == null) return NotFound();
            return View(hotel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var (success, message) = await _hotelService.DeleteAsync(id);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, string status)
        {
            var (success, message) = await _hotelService.ChangeStatusAsync(id, status);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction(nameof(Index));
        }
    }
}