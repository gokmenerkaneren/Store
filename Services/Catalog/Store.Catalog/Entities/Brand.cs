using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Store.Shared.Entities;
using Store.Shared.Enums;

namespace Store.Catalog.Entities
{
    // =========================================================================
    // BRAND (Marka Bilgileri)
    // Marka ile ilgili temel, kimlik ve tanım bilgilerini tutar.
    // =========================================================================
    public class Brand : BaseEntity
    {
        /// Markanın kısa kodu veya slug'ı.
        public string Code { get; set; } = string.Empty;

        /// Markanın tam adı (Örn: Sezadan Dekoratif).
        public string Name { get; set; } = string.Empty;
    }
}

