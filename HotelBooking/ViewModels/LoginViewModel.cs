using System.ComponentModel.DataAnnotations;

namespace HotelBooking.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email hoặc Username không được để trống")]
        [Display(Name = "Email hoặc Username")]
        public string EmailOrUserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool RememberMe { get; set; }
    }
}