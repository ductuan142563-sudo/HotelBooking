using HotelBooking.Models;
using HotelBooking.Repositories;
using HotelBooking.Services;
using HotelBooking.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoomsController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IRoomTypeRepository _roomTypeRepo;
        private readonly IHotelRepository _hotelRepo; // giả sử đã có từ PART 5

        public RoomsController(
            IRoomService roomService,
            IRoomTypeRepository roomTypeRepo,
            IHotelRepository hotelRepo)
        {
            _roomService = roomService;
            _roomTypeRepo = roomTypeRepo;
            _hotelRepo = hotelRepo;
        }

        public async Task<IActionResult> Index(string? searchTerm, int? hotelId, int? roomTypeId,
            RoomStatus? status, int page = 1)
        {
            const int pageSize = 10;
            var (rooms, totalCount) = await _roomService.SearchAsync(
                searchTerm, hotelId, roomTypeId, status, page, pageSize);

            var model = new RoomListViewModel
            {
                Rooms = rooms,
                SearchTerm = searchTerm,
                HotelId = hotelId,
                RoomTypeId = roomTypeId,
                Status = status,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageSize = pageSize,
                Hotels = new SelectList(await _hotelRepo.GetAllAsync(), "HotelId", "Name"),
                RoomTypes = new SelectList(await _roomTypeRepo.GetAllAsync(), "RoomTypeId", "Name")
            };

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var model = new RoomViewModel
            {
                Hotels = new SelectList(await _hotelRepo.GetAllAsync(), "HotelId", "Name"),
                RoomTypes = new SelectList(await _roomTypeRepo.GetAllAsync(), "RoomTypeId", "Name")
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Hotels = new SelectList(await _hotelRepo.GetAllAsync(), "HotelId", "Name");
                model.RoomTypes = new SelectList(await _roomTypeRepo.GetAllAsync(), "RoomTypeId", "Name");
                return View(model);
            }

            var (success, message) = await _roomService.CreateAsync(model);
            if (success)
            {
                TempData["Success"] = message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", message);
            model.Hotels = new SelectList(await _hotelRepo.GetAllAsync(), "HotelId", "Name");
            model.RoomTypes = new SelectList(await _roomTypeRepo.GetAllAsync(), "RoomTypeId", "Name");
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var room = await _roomService.GetByIdAsync(id);
            if (room == null) return NotFound();

            var model = new RoomViewModel
            {
                RoomId = room.RoomId,
                HotelId = room.HotelId,
                RoomTypeId = room.RoomTypeId,
                RoomNumber = room.RoomNumber,
                Price = room.Price,
                Capacity = room.Capacity,
                Floor = room.Floor,
                Description = room.Description,
                Status = room.Status,
                ExistingImage = room.Image,
                Hotels = new SelectList(await _hotelRepo.GetAllAsync(), "HotelId", "Name", room.HotelId),
                RoomTypes = new SelectList(await _roomTypeRepo.GetAllAsync(), "RoomTypeId", "Name", room.RoomTypeId)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoomViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Hotels = new SelectList(await _hotelRepo.GetAllAsync(), "HotelId", "Name");
                model.RoomTypes = new SelectList(await _roomTypeRepo.GetAllAsync(), "RoomTypeId", "Name");
                return View(model);
            }

            var (success, message) = await _roomService.UpdateAsync(model);
            if (success)
            {
                TempData["Success"] = message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", message);
            model.Hotels = new SelectList(await _hotelRepo.GetAllAsync(), "HotelId", "Name");
            model.RoomTypes = new SelectList(await _roomTypeRepo.GetAllAsync(), "RoomTypeId", "Name");
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var room = await _roomService.GetByIdAsync(id);
            if (room == null) return NotFound();
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, message) = await _roomService.DeleteAsync(id);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, RoomStatus status)
        {
            var (success, message) = await _roomService.ChangeStatusAsync(id, status);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction(nameof(Index));
        }
    }
}