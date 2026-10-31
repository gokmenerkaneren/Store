using System;
using System.ComponentModel.DataAnnotations;

namespace Store.WebUI.Models.HesabimViewModels
{
    public class KullaniciBilgileriViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Ad")]
        [Required(ErrorMessage = "Lütfen adınızı giriniz.")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Soyad")]
        [Required(ErrorMessage = "Lütfen soyadınızı giriniz.")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "E-posta Adresi")]
        [Required(ErrorMessage = "Lütfen e-posta adresinizi giriniz.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Cep Telefonu")]
        [Required(ErrorMessage = "Lütfen cep telefonu numaranızı giriniz.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "T.C. Kimlik No")]
        public string? TCKimlik { get; set; }

        [Display(Name = "İş yeri alışverişlerim için kurumsal fırsatlardan haberdar olmak istiyorum.")]
        public bool AcceptCorporateOffers { get; set; } = false;

        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
    }
}

