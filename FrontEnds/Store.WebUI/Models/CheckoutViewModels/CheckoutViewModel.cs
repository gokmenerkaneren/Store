using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Store.WebUI.Models.CheckoutViewModels
{
    public class CheckoutViewModel
    {
        // 1. İletişim (Contact)
        [Display(Name = "E-posta veya Cep Telefonu")]
        [Required(ErrorMessage = "Lütfen e-posta veya telefon numarası giriniz.")]
        public string EmailOrPhone { get; set; } = string.Empty;

        [Display(Name = "Haberler ve özel teklifler için bana e-posta gönder")]
        public bool AcceptMarketing { get; set; } = true;

        // 2. Teslimat Adresi (Delivery)
        [Display(Name = "Ülke / Bölge")]
        public string Country { get; set; } = "Türkiye";

        [Display(Name = "Ad (isteğe bağlı)")]
        public string? FirstName { get; set; }

        [Display(Name = "Soyad")]
        [Required(ErrorMessage = "Lütfen soyadınızı giriniz.")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Adres")]
        [Required(ErrorMessage = "Lütfen adres bilginizi giriniz.")]
        public string AddressLine { get; set; } = string.Empty;

        [Display(Name = "Daire, süit vb. (isteğe bağlı)")]
        public string? Apartment { get; set; }

        [Display(Name = "Posta Kodu (isteğe bağlı)")]
        public string? PostalCode { get; set; }

        [Display(Name = "İl")]
        [Required(ErrorMessage = "Lütfen şehir/il seçiniz.")]
        public string City { get; set; } = string.Empty;

        [Display(Name = "İlçe")]
        [Required(ErrorMessage = "Lütfen ilçe giriniz.")]
        public string District { get; set; } = string.Empty;

        [Display(Name = "Telefon")]
        public string? Phone { get; set; }

        [Display(Name = "Bu bilgileri bir sonraki sefer için kaydet")]
        public bool SaveAddressForNextTime { get; set; } = true;

        // 3. Kargo Yöntemi (Shipping Method)
        public string ShippingMethodTitle { get; set; } = "Standart Teslimat (1-3 İş Günü)";
        public decimal ShippingFee { get; set; } = 0.00m;

        // 4. Ödeme Yöntemi (Payment)
        public string PaymentMethod { get; set; } = "CreditCard"; // CreditCard, BankTransfer, CashOnDelivery

        [Display(Name = "Kart Üzerindeki İsim")]
        public string? CardHolderName { get; set; }

        [Display(Name = "Kart Numarası")]
        public string? CardNumber { get; set; }

        [Display(Name = "Son Kullanma Ay")]
        public string? ExpireMonth { get; set; }

        [Display(Name = "Son Kullanma Yıl")]
        public string? ExpireYear { get; set; }

        [Display(Name = "Güvenlik Kodu (CVV)")]
        public string? Cvv { get; set; }

        // Sipariş Notu
        public string? OrderNote { get; set; }

        // Kupon / İndirim Kodu
        public string? DiscountCode { get; set; }

        // 5. Sipariş Özeti (Line Items & Totals)
        public List<CheckoutItemViewModel> Items { get; set; } = new();

        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal => Math.Max(0, (SubTotal - DiscountAmount) + ShippingFee);
    }

    public class CheckoutItemViewModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice => Quantity * Price;
    }
}

