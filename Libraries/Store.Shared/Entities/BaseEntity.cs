using System;
using System.ComponentModel.DataAnnotations;

namespace Store.Shared.Entities
{
    /// <summary>
    /// SİSTEM TEMEL VARLIĞI (BASE ENTITY)
    /// Tüm mikroservislerdeki entity'lerin ortak loglama, durum ve takip alanlarını barındıran temel sınıftır.
    /// Mikroservisler arası uyum için ID değeri GUID olarak belirlenmiştir.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Tablo bazında benzersiz kimliği (Primary Key).
        /// </summary>
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Bu kayda ait özel durumlar, açıklamalar veya sistem yöneticisi notları.
        /// </summary>
        [StringLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Kaydın sistemde aktif olarak kullanılıp kullanılmadığını belirtir.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Bu kaydı veritabanına ilk ekleyen kullanıcının veya sistemin benzersiz ID'si.
        /// </summary>
        public Guid InsertUserId { get; set; }

        /// <summary>
        /// Bu kaydın veritabanına ilk eklendiği tarih ve saat (Zaman dilimi destekli).
        /// </summary>
        public DateTimeOffset InsertDate { get; set; } = GetLocalTime();

        /// <summary>
        /// Bu kaydı en son güncelleyen kullanıcının benzersiz ID'si.
        /// </summary>
        public Guid? UpdateUserId { get; set; }

        /// <summary>
        /// Bu kaydın en son güncellendiği tarih ve saat.
        /// </summary>
        public DateTimeOffset? UpdateDate { get; set; }

        /// <summary>
        /// Kaydın sistemden silinip silinmediğini belirtir (Soft Delete mantığı).
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Bu kaydı en son silen (Soft Delete yapan) kullanıcının ID'si.
        /// </summary>
        public Guid? DeleteUserId { get; set; }

        /// <summary>
        /// Bu kaydın en son silindiği tarih ve saat.
        /// </summary>
        public DateTimeOffset? DeleteDate { get; set; }

        /// <summary>
        /// PROGRAM GENELİ TARİH/SAAT YAPISI:
        /// Bilgisayarın yerel tarih ve saatini UTC dönüşümüne uğramadan veritabanına kaydetmek için oluşturulmuş yapı.
        /// PostgreSQL veya MongoDB'nin otomatik UTC çevirisi yapıp saati geri almasını engeller.
        /// </summary>
        public static DateTimeOffset GetLocalTime()
        {
            return new DateTimeOffset(DateTime.Now.Ticks, TimeSpan.Zero);
        }
    }
}
