using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Location.Domain.Entities
{
    // ==========================================
    // 1. LOKASYON BİLGİLERİ
    // ==========================================

    /// Sistemde kayıtlı olan ülkeleri tutar.
    public class Country : BaseEntity
    {
        /// Ülkenin resmi tam adı (Örn: Türkiye, Almanya).
        [Required, StringLength(100)]
        public string CountryName { get; set; } = string.Empty;

        /// Uluslararası standart 2 veya 3 haneli ülke kodu (Örn: TR, DE).
        [StringLength(5)]
        public string? CountryCode { get; set; }

        /// Uluslararası standart ülke telefon ön kodu (Örn: +90).
        [StringLength(10)]
        public string? UPhoneCode { get; set; }

        /// Bu ülkeye bağlı olan tüm şehirlerin listesi (Bire-Çok İlişki Navigation Property).
        public ICollection<City> Cities { get; set; } = new List<City>();
    }

}
