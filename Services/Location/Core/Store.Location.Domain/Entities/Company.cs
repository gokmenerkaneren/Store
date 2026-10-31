using System;
using Store.Shared.Enums;
using System.Collections.Generic;
using Store.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using Store.Shared.Enums;
using System.Linq;
using Store.Shared.Enums;
using System.Text;
using Store.Shared.Enums;
using System.Threading.Tasks;
using Store.Shared.Enums;

namespace Store.Location.Domain.Entities
{
    /// Sistemi kullanan ana şirketlerin (veya alt şirketlerin) kurumsal ana verilerini tutar.
    public class Company : BaseEntity
    {
        /// Şirketin faturalardaki resmi ticari unvanı.
        [Required, StringLength(250)]
        public string LegalName { get; set; } = string.Empty;

        /// Şirketin piyasada ve sitede bilinen marka adı.
        [Required, StringLength(250)]
        public string BrandName { get; set; } = string.Empty;

        /// Şirketin bağlı olduğu Vergi Dairesi.
        [StringLength(100)]
        public string TaxOffice { get; set; } = string.Empty;

        /// Şirketin Vergi Kimlik Numarası (VKN).
        [StringLength(15)]
        public string TaxNumber { get; set; } = string.Empty;

        /// Şirketin resmi Ticaret Sicil Numarası.
        [StringLength(100)]
        public string TradeRegistryNumber { get; set; } = string.Empty;

        /// Şirketin merkezi sicil kayıt sistemi (MERSİS) numarası.
        [StringLength(30)]
        public string MersisNumber { get; set; } = string.Empty;

        /// Şirketin kurumsal web sitesi adresi (URL).
        [StringLength(200)]
        public string Website { get; set; } = string.Empty;

        /// Şirket logosunun sunucu veya CDN üzerindeki bağlantı yolu.
        [StringLength(500)]
        public string LogoUrl { get; set; } = string.Empty;

        /// Şirket merkezine ait iletişim kişisi.
        [StringLength(50)]
        public string MainContactName { get; set; } = string.Empty;

        /// Şirket email adresi.
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        /// Şirket telefon numarası.
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        // ==========================================
        // E-TİCARET & B2B / MUHASEBE ALANLARI
        // ==========================================

        /// Fatura Tipi: Bireysel (Şahıs) veya Kurumsal.
        public InvoiceType InvoiceType { get; set; }

        /// Şirket e-fatura mükellefi mi?
        public bool IsEInvoiceUser { get; set; }

        /// ERP (Logo, Mikro, SAP vb.) sistemlerindeki cari hesap entegrasyon kodu.
        [StringLength(50)]
        public string ErpCode { get; set; } = string.Empty;

        /// Firmanın vergi dairesinin bağlı olduğu Ülke ID'si (Global operasyonlar için).
        public Guid? CountryId { get; set; }

        /// Şirketin resmi fatura adresinin kimliği (İleride Address tablosuna FK olabilir).
        public Guid? BillingAddressId { get; set; }
    }

}
