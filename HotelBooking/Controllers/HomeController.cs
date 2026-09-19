using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HotelBooking.Models; // sẽ tạo sau, tạm thời comment nếu lỗi

namespace HotelBooking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Title"] = "Giới thiệu";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Title"] = "Liên hệ";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}