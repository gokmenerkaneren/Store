using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Location.Domain.Entities
{
    /// Sistemde kayıtlı olan şehirleri tutar. Lojistik hesaplamaları için kritiktir.
    public class City : BaseEntity
    {
        /// Şehrin adı (Örn: İzmir, İstanbul).
        [Required, StringLength(100)]
        public string CityName { get; set; } = string.Empty;

        /// Şehrin trafik plaka kodu (Örn: 35). Uluslararası sistemler için boş geçilebilir.
        [StringLength(10)]
        public string? PlateCode { get; set; }

        /// Şehrin bağlı olduğu ülkenin benzersiz kimliği (Foreign Key).
        public Guid CountryId { get; set; }

        /// Şehrin bağlı olduğu Ülke nesnesine doğrudan erişim (Navigation Property).
        public Country? Country { get; set; }

        /// Bu şehre bağlı olan tüm ilçelerin listesi (Bire-Çok İlişki Navigation Property).
        public ICollection<District> Districts { get; set; } = new List<District>();
    }

}
