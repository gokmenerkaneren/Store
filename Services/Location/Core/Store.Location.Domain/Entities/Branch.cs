using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Location.Domain.Entities
{
    /// <summary>
    /// Ana şirkete bağlı faaliyet gösteren şubeleri (Mağaza, Depo, Üretim tesisi) tutar.
    /// Her şubenin kendi iletişim kaydı (ContactInfo) ve adresi (BranchAddress) vardır.
    /// </summary>
    public class Branch : BaseEntity
    {
        /// Şubenin genel adı (Örn: Karşıyaka Merkez Mağaza).
        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;

        /// Şubenin faaliyet türü (Örn: Mağaza, Depo, Üretim).
        [StringLength(100)]
        public string BranchType { get; set; } = string.Empty;

        /// Bu şubenin hangi ana Şirkete ait olduğunun ID'si (Foreign Key).
        public Guid CompanyId { get; set; }

        /// Şubenin bağlı olduğu Şirket nesnesine erişim (Navigation Property).
        public Company? Company { get; set; }

        /// Bu şubenin iletişim bilgilerini tutan kaydın ID'si (Foreign Key).
        public Guid? ContactInfoId { get; set; }

        /// Şubenin iletişim nesnesine erişim — Navigation Property (BUG FIX: daha önce eksikti).
        public ContactInfo? ContactInfo { get; set; }

        /// Bu şubenin harita/koordinat bilgilerini enlem ve boylam (Lat,Long) formatında tutar.
        [StringLength(100)]
        public string? GeoLocationCoordinates { get; set; }
    }
}

