using System.ComponentModel.DataAnnotations;

namespace Store.WebUI.Models.HesabimViewModels
{
    public class SifreDegistirmeViewModel
    {
        [Display(Name = "Mevcut Şifre")]
        [Required(ErrorMessage = "Lütfen mevcut şifrenizi giriniz.")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Display(Name = "Yeni Şifre")]
        [Required(ErrorMessage = "Lütfen yeni şifrenizi giriniz.")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter olmalıdır.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Display(Name = "Yeni Şifre (Tekrar)")]
        [Required(ErrorMessage = "Lütfen yeni şifrenizi tekrar giriniz.")]
        [Compare("NewPassword", ErrorMessage = "Yeni şifreler birbiriyle uyuşmuyor.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
    }
}

