using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Store.Shared.Entities;
using Store.Shared.Enums;

namespace Store.Catalog.Entities
{
    // =========================================================================
    // 4. CATEGORY (Kategori Bilgileri)
    // Kategori ile ilgili temel ve hiyerarşi bilgilerini tutar.
    // =========================================================================
    public class Category : BaseEntity
    {
        /// Eğer bu bir alt kategori ise, bağlı olduğu üst kategorinin kimliği. Compass'ta okunabilirlik için String olarak tutulur.
        public Guid? ParentId { get; set; }

        /// Kategorinin sistem veya ERP entegrasyonu için kısa kodu.
        public string? Code { get; set; } = string.Empty;

        /// Kategorinin sitede görünecek tam adı (Örn: Nişan Konseptleri).
        public string Name { get; set; } = string.Empty;

        /// Kategoriye ait görselin sunucudaki yolu (opsiyonel, null olabilir).
        public string? ImageUrl { get; set; }
    }
}

