using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Location.Domain.Entities
{
    /// <summary>
    /// B2B sistemi için toptan alım/kiralama yapan Bayi profillerini tutar.
    /// Finansal ve ticari veriler (iskonto, kredi limiti, bakiye) bu sınıftadır.
    /// Fiziksel adres bilgisi DealerAddress entity'si üzerinden yönetilir.
    /// </summary>
    public class Dealer : BaseEntity
    {
        /// Bayinin ticari unvanı veya şahıs şirket ismi.
        [Required, StringLength(250)]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(50)]
        public string TaxNumber { get; set; } = string.Empty;

        [StringLength(100)]
        public string TaxOffice { get; set; } = string.Empty;

        public decimal DiscountRate { get; set; }
        
        public decimal CreditLimit { get; set; }
        
        public decimal CurrentBalance { get; set; }
        // ==========================================
        // RELATIONS (İLİŞKİLER)
        // ==========================================

        /// Bayinin iletişim bilgilerini tutan kaydın ID'si (Foreign Key).
        public Guid? ContactInfoId { get; set; }

        /// Bayinin iletişim nesnesine erişim (Navigation Property).
        public ContactInfo? ContactInfo { get; set; }

        /// Bayiye ait alt şubelerin listesi (1:N İlişki).
        public ICollection<Branch> Branches { get; set; } = new List<Branch>();

        /// Bayiye ait fiziksel adres kayıtlarının listesi (1:N İlişki — Merkez, Fatura, Sevkiyat).
        public ICollection<DealerAddress> Addresses { get; set; } = new List<DealerAddress>();
    }
}

