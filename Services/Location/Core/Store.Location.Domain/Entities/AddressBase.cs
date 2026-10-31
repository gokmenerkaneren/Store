using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Location.Domain.Entities
{
    /// <summary>
    /// TPC (Table-Per-Concrete-Type) Mimarisi için Adres Şablonu.
    /// Veritabanında tablo olarak oluşmaz; BranchAddress, UserAddress, DealerAddress
    /// gibi alt sınıflara kalıtım yoluyla tüm lokasyon FK'larını aktarır.
    /// </summary>
    public abstract class AddressBase : BaseEntity
    {
        /// Adres için kullanıcının belirlediği başlık (Örn: Merkez, Fatura Adresi, Depo).
        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;

        /// Bu adreste teslimatın veya iletişimin yapılacağı kişinin tam adı.
        [Required, StringLength(150)]
        public string ContactFullName { get; set; } = string.Empty;

        /// Bu adrese ait kargo/teslimat iletişim numarası.
        [Required, StringLength(30)]
        public string PhoneNumber { get; set; } = string.Empty;

        /// Sokak, Cadde, Bina No, Daire gibi açık adres detayları.
        [Required, StringLength(500)]
        public string AddressDetail { get; set; } = string.Empty;

        // ==========================================
        // LOKASYON HİYERARŞİSİ (Dropdown Tabanlı)
        // ==========================================

        /// Adresin bağlı bulunduğu Ülkenin kimliği (Foreign Key).
        public Guid CountryId { get; set; }

        /// Adresin bağlı bulunduğu Ülke nesnesi (Navigation Property).
        public Country? Country { get; set; }

        /// Adresin bağlı bulunduğu Şehrin kimliği (Foreign Key).
        public Guid CityId { get; set; }

        /// Adresin bağlı bulunduğu Şehir nesnesi (Navigation Property).
        public City? City { get; set; }

        /// Adresin bağlı bulunduğu İlçenin kimliği (Foreign Key).
        public Guid DistrictId { get; set; }

        /// Adresin bağlı bulunduğu İlçe nesnesi (Navigation Property).
        public District? District { get; set; }

        /// Adresin bağlı bulunduğu Mahallenin kimliği (Nullable — her ülkede zorunlu değil).
        public Guid? NeighborhoodId { get; set; }

        /// Adresin bağlı bulunduğu Mahalle nesnesi (Navigation Property).
        public Neighborhood? Neighborhood { get; set; }
    }
}
