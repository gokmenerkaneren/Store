using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Store.WebUI.Controllers
{
    public class RequirementViewModel
    {
        [Required(ErrorMessage = "Tarih alanı zorunludur.")]
        [DataType(DataType.Date)]
        public DateTime? DateField { get; set; }

        [Required(ErrorMessage = "Tarih ve Saat alanı zorunludur.")]
        [DataType(DataType.DateTime)]
        public DateTime? DateTimeField { get; set; }

        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Lütfen geçerli bir e-posta adresi formatı (örn: ornek@gmail.com) giriniz.")]
        public string EmailField { get; set; }

        [Required(ErrorMessage = "Cep telefonu alanı zorunludur.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [RegularExpression(@"^\(\d{3}\) \d{3} \d{2} \d{2}$", ErrorMessage = "Geçerli bir telefon numarası (örn: (555) 444 33 22) giriniz.")]
        public string PhoneField { get; set; }

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Şifre en az 8 karakter olmalıdır.")]
        public string PasswordField { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı alanı zorunludur.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Kullanıcı adı sadece harf, rakam ve alt çizgi içerebilir.")]
        public string UsernameField { get; set; }

        [Required(ErrorMessage = "Metin alanı zorunludur.")]
        [RegularExpression(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$", ErrorMessage = "Sadece harf girebilirsiniz.")]
        public string TextOnlyField { get; set; }

        [Required(ErrorMessage = "Rakam alanı zorunludur.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Sadece rakam girebilirsiniz.")]
        public string NumberOnlyField { get; set; }

        [Required(ErrorMessage = "Lütfen bir seçenek belirleyiniz.")]
        public string DropdownField { get; set; }
    }

    public class ZZRequirementsController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new RequirementViewModel());
        }

        [HttpPost]
        public IActionResult Index(RequirementViewModel model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.SuccessMessage = "Tüm alanlar başarıyla doğrulandı ve form gönderildi!";
                return View(new RequirementViewModel()); // Reset form on success
            }
            
            return View(model);
        }
    }
}
