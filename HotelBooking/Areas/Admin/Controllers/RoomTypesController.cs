using HotelBooking.Services;
using HotelBooking.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoomTypesController : Controller
    {
        private readonly IRoomTypeService _service;

        public RoomTypesController(IRoomTypeService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _service.GetAllAsync();
            return View(list);
        }

        public IActionResult Create()
        {
            return View(new RoomTypeViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomTypeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (success, message) = await _service.CreateAsync(model);
            if (success)
            {
                TempData["Success"] = message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", message);
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var roomType = await _service.GetByIdAsync(id);
            if (roomType == null) return NotFound();

            var model = new RoomTypeViewModel
            {
                RoomTypeId = roomType.RoomTypeId,
                Name = roomType.Name,
                Description = roomType.Description,
                MaxGuests = roomType.MaxGuests,
                BedType = roomType.BedType,
                Area = roomType.Area,
                BasePrice = roomType.BasePrice,
                ExistingImage = roomType.Image,
                IsActive = roomType.IsActive
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoomTypeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (success, message) = await _service.UpdateAsync(model);
            if (success)
            {
                TempData["Success"] = message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", message);
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var roomType = await _service.GetByIdAsync(id);
            if (roomType == null) return NotFound();
            return View(roomType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, message) = await _service.DeleteAsync(id);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction(nameof(Index));
        }
    }
}