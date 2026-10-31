using System;
using Store.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Store.Location.Domain.Entities
{
    /// Sistemdeki Müşterilere (B2C veya B2B) ait fatura ve teslimat adres detaylarını tutar.
    public class UserAddress : AddressBase
    {
        /// Bu adresin hangi müşteriye ait olduğunu belirten kimlik (Foreign Key).
        public Guid CustomerId { get; set; }

        /// Fatura türünü belirler (Bireysel veya Kurumsal).
        public InvoiceType InvoiceType { get; set; }

        /// Bireysel faturalar için T.C. Kimlik Numarası (Şifrelenmiş olarak saklanmalıdır).
        [StringLength(11)]
        public string? IdentityNumber { get; set; }

        /// Kurumsal faturalar için şirketin bağlı olduğu Vergi Dairesi.
        [StringLength(100)]
        public string? TaxOffice { get; set; }

        /// Kurumsal faturalar için şirketin Vergi Numarası.
        [StringLength(50)]
        public string? TaxNumber { get; set; }

        /// Kurumsal faturalar için faturanın kesileceği şirketin/bayinin kimliği.
        public Guid? CompanyId { get; set; }

        /// Kullanıcı hesabını sildiğinde KVKK gereği adres verilerinin anonimleştirilip anonimleştirilmediği flag'i.
        public bool IsAnonymized { get; set; }

        /// Bu adresin kullanıcının varsayılan teslimat adresi olup olmadığını belirler.
        public bool IsDefault { get; set; } = false;

        /// KVKK (Kişisel Verilerin Korunması Kanunu) gereği kişisel verileri maskeler.
        public void AnonymizeData()
        {
            ContactFullName = "*** ***";
            PhoneNumber = "0000000000";
            AddressDetail = "KVKK gereği silinmiştir.";
            IdentityNumber = null;
            IsAnonymized = true;
        }
    }
}
