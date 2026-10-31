using System;
using System.ComponentModel.DataAnnotations;

namespace Store.WebUI.Models.HesabimViewModels
{
    public class UserAddressViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Adres Başlığı")]
        [Required(ErrorMessage = "Lütfen adres başlığı giriniz (Örn: Ev, İş).")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Ad")]
        public string? FirstName { get; set; }

        [Display(Name = "Soyad")]
        public string? LastName { get; set; }

        [Display(Name = "Ad Soyad")]
        [Required(ErrorMessage = "Lütfen teslim alacak kişinin adını ve soyadını giriniz.")]
        public string ContactFullName { get; set; } = string.Empty;

        [Display(Name = "Telefon Numarası")]
        [Required(ErrorMessage = "Lütfen iletişim telefonunu giriniz.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Ülke")]
        [Required(ErrorMessage = "Lütfen ülke seçiniz.")]
        public Guid CountryId { get; set; } = Guid.Parse("dbcf7622-7a82-4439-84d6-ff571b8ef053");
        public string CountryName { get; set; } = "Türkiye";

        [Display(Name = "İl")]
        [Required(ErrorMessage = "Lütfen il seçiniz.")]
        public Guid CityId { get; set; }
        public string CityName { get; set; } = string.Empty;

        [Display(Name = "İlçe")]
        [Required(ErrorMessage = "Lütfen ilçe seçiniz.")]
        public Guid DistrictId { get; set; }
        public string DistrictName { get; set; } = string.Empty;

        [Display(Name = "Mahalle")]
        public Guid? NeighborhoodId { get; set; }
        public string? NeighborhoodName { get; set; }

        [Display(Name = "Açık Adres")]
        [Required(ErrorMessage = "Lütfen açık adres bilginizi giriniz.")]
        public string AddressDetail { get; set; } = string.Empty;

        [Display(Name = "Fatura Türü")]
        public int InvoiceType { get; set; } = 0; // 0: Bireysel, 1: Kurumsal

        [Display(Name = "T.C. Kimlik Numarası")]
        public string? IdentityNumber { get; set; }

        [Display(Name = "Vergi Dairesi")]
        public string? TaxOffice { get; set; }

        [Display(Name = "Vergi Numarası")]
        public string? TaxNumber { get; set; }

        [Display(Name = "Firma Unvanı")]
        public string? CompanyTitle { get; set; }

        [Display(Name = "Varsayılan Teslimat Adresi")]
        public bool IsDefault { get; set; } = false;
    }
}
