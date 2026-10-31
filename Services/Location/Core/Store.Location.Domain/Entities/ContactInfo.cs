using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Location.Domain.Entities
{
    /// <summary>
    /// Bayi, Şube, Şirket veya Depolara ait kurumsal iletişim ve yetkili kişi bilgilerini tutar.
    /// Fiziksel adres bilgisi bu sınıfın sorumluluğunda DEĞİLDİR.
    /// Adres yönetimi AddressBase'den türeyen DealerAddress, BranchAddress vb. sınıflar tarafından yapılır.
    /// </summary>
    public class ContactInfo : BaseEntity
    {
        // ==========================================
        // 1. KURUMSAL İLETİŞİM BİLGİLERİ
        // ==========================================

        /// Kurumsal e-posta adresi (Örn: info@bayi.com).
        [StringLength(150)]
        public string CompanyEmail { get; set; } = string.Empty;

        /// Kurumsal sabit hat / santral numarası.
        [StringLength(30)]
        public string Phone { get; set; } = string.Empty;

        /// Kurumsal faks numarası.
        [StringLength(30)]
        public string Fax { get; set; } = string.Empty;

        /// Kurumsal web sitesi adresi.
        [StringLength(100)]
        public string Website { get; set; } = string.Empty;

        // ==========================================
        // 2. YETKİLİ KİŞİ (KONTAK) BİLGİLERİ
        // ==========================================

        /// Bu iletişim noktasıyla görüşülecek yetkili kişinin tam adı ve soyadı.
        [StringLength(100)]
        public string AuthorizedPersonName { get; set; } = string.Empty;

        /// Yetkili kişinin unvanı (Örn: Satınalma Müdürü, Mağaza Sorumlusu).
        [StringLength(100)]
        public string AuthorizedPersonTitle { get; set; } = string.Empty;

        /// Yetkili kişinin direkt cep telefonu veya dahili numarası.
        [StringLength(30)]
        public string AuthorizedPersonPhone { get; set; } = string.Empty;

        /// Yetkili kişinin direkt e-posta adresi (KVKK kapsamında kişisel veri).
        [StringLength(150)]
        public string AuthorizedPersonEmail { get; set; } = string.Empty;
    }
}
