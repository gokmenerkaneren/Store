using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Store.Shared.Entities;
using Store.Shared.Enums;

namespace Store.Catalog.Entities
{
    // =========================================================================
    // CURRENCY (Para Birimi)
    // Para birimlerinin kod, isim ve sembol bilgisini tutar.
    // =========================================================================
    public class Currency : BaseEntity
    {
        /// Para biriminin uluslararası 3 haneli ISO kodu (Örn: TRY, USD).
        public string Code { get; set; } = string.Empty;

        /// Para biriminin okunuş adı (Örn: Türk Lirası).
        public string Name { get; set; } = string.Empty;

        /// Para biriminin grafiksel sembolü (Örn: ₺, $).
        public string Symbol { get; set; } = string.Empty;
    }
}

