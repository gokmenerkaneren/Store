using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Store.Shared.Entities;
using Store.Shared.Enums;

namespace Store.Catalog.Entities
{
    // =========================================================================
    // MEASUREMENT (Ölçü Birimi)
    // Ürün miktarlarını tanımlamak için kullanılan ölçü birimlerinin kod ve isim bilgileri.
    // =========================================================================
    public class Measurement : BaseEntity
    {
        /// Ölçü biriminin sistem kodu.
        public string Code { get; set; } = string.Empty;

        /// Ölçü biriminin tam okunuşu (Örn: Kilogram).
        public string Name { get; set; } = string.Empty;

        /// Ölçü biriminin kısa sembolü (Örn: Kg).
        public string Symbol { get; set; } = string.Empty;
    }
}

