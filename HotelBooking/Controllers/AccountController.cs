using HotelBooking.Models;
using HotelBooking.Services;
using HotelBooking.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;

namespace HotelBooking.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _logger = logger;
        }

        // ===================== REGISTER =====================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            // Kiểm tra trùng Email
            var existingEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError(nameof(model.Email), "Email đã được sử dụng.");
                return View(model);
            }

            // Kiểm tra trùng Username
            var existingUserName = await _userManager.FindByNameAsync(model.UserName);
            if (existingUserName != null)
            {
                ModelState.AddModelError(nameof(model.UserName), "Username đã tồn tại.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = true,          // Demo: không cần xác nhận email
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Gán Role Customer mặc định
                await _userManager.AddToRoleAsync(user, "Customer");

                _logger.LogInformation("User mới đăng ký: {UserName}", user.UserName);

                // Tự động đăng nhập
                await _signInManager.SignInAsync(user, isPersistent: false);

                TempData["Success"] = "Đăng ký thành công! Chào mừng bạn đến với Hotel Booking.";
                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // ===================== LOGIN =====================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            // Tìm user bằng Email hoặc Username
            ApplicationUser? user = null;

            if (model.EmailOrUserName.Contains("@"))
                user = await _userManager.FindByEmailAsync(model.EmailOrUserName);
            else
                user = await _userManager.FindByNameAsync(model.EmailOrUserName);

            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Email/Username hoặc mật khẩu không đúng.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("User đăng nhập: {UserName}", user.UserName);
                TempData["Success"] = $"Xin chào {user.FullName}!";
                return LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("Tài khoản bị khóa: {UserName}", user.UserName);
                ModelState.AddModelError(string.Empty, "Tài khoản đã bị khóa tạm thời do đăng nhập sai quá nhiều lần. Vui lòng thử lại sau.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Email/Username hoặc mật khẩu không đúng.");
            return View(model);
        }

        // ===================== LOGOUT =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User đã đăng xuất.");
            TempData["Success"] = "Bạn đã đăng xuất thành công.";
            return RedirectToAction("Index", "Home");
        }

        // ===================== FORGOT PASSWORD =====================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            // Luôn trả về trang confirmation (bảo mật – không tiết lộ email có tồn tại hay không)
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // Tạo Reset Token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var callbackUrl = Url.Action(
                "ResetPassword",
                "Account",
                new { email = user.Email, token = encodedToken },
                protocol: Request.Scheme);

            var emailBody = $@"
                <h2>Đặt lại mật khẩu - Hotel Booking</h2>
                <p>Xin chào {user.FullName},</p>
                <p>Bạn vừa yêu cầu đặt lại mật khẩu. Vui lòng nhấn vào link bên dưới:</p>
                <p><a href='{HtmlEncoder.Default.Encode(callbackUrl!)}' 
                      style='padding:10px 20px;background:#0d6efd;color:white;text-decoration:none;border-radius:5px;'>
                      Đặt lại mật khẩu
                   </a></p>
                <p>Hoặc copy link: {callbackUrl}</p>
                <p>Link này có hiệu lực trong thời gian ngắn. Nếu bạn không yêu cầu, hãy bỏ qua email này.</p>
                <hr/>
                <p><small>Hotel Booking System</small></p>";

            await _emailService.SendEmailAsync(user.Email!, "Đặt lại mật khẩu - Hotel Booking", emailBody);

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // ===================== RESET PASSWORD =====================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string? email = null, string? token = null)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Link đặt lại mật khẩu không hợp lệ.");
            }

            var model = new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Không tiết lộ thông tin
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            // Decode token
            string decodedToken;
            try
            {
                decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Token không hợp lệ.");
                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserName} đã đặt lại mật khẩu thành công.", user.UserName);
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        // ===================== ACCESS DENIED =====================
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}