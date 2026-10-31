using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Store.Shared.Entities;
using Store.Shared.Enums;

namespace Store.Catalog.Entities
{
    // ==========================================
    // 2. ÜRÜN KARTLARI VE DETAYLARI
    // ==========================================

    /// Ürünün satış, kiralama, perakende veya toptan olmak üzere dönemsel fiyat listelerini tutar.
    public class ProductPrice : BaseEntity
    {
        /// Bu fiyat bilgisinin bağlı olduğu ana Ürünün ID'si.
        [BsonRepresentation(BsonType.String)]
        public Guid ProductId { get; set; }

        /// Bu fiyatın hangi para birimi (TRY, USD) üzerinden olduğunu belirten ID.
        [BsonRepresentation(BsonType.String)]
        public Guid CurrencyId { get; set; }

        /// Fiyatın türünü (Maliyet, Perakende, Kiralama, Toptan vs.) belirten PriceType koleksiyonunun ID'si.
        [BsonRepresentation(BsonType.String)]
        public Guid PriceTypeId { get; set; }

        /// B2B sistemi için bu fiyatın hangi "Fiyat Listesi"ne (Örn: Gold Bayi, 2026 Kampanyası) özel olduğunu belirtir. Null ise standart fiyat listesidir.
        [BsonRepresentation(BsonType.String)]
        public Guid? PriceListId { get; set; }

        /// Türetildiği temel fiyatın referansı (Örn: Toptan Fiyatın ID'si). Null ise bağımsız bir fiyattır.
        [BsonRepresentation(BsonType.String)]
        public Guid? BasePriceId { get; set; }

        /// Temel fiyattan türetilirken üzerine eklenen veya çıkarılan kâr/zarar marjı (%)
        public decimal? MarginPercentage { get; set; }

        /// Toplu alım indirimleri için, bu fiyatın geçerli olması gereken asgari sepet miktarı.
        public int MinimumQuantity { get; set; } = 1;

        /// Ürünün geçerli olan asıl satış/kiralama tutarı (Örn: 1000 TL).
        public decimal Amount { get; set; }

        /// Eğer aktif bir kampanya varsa, ürünün düşmüş indirimli tutarı.
        public decimal? DiscountedAmount { get; set; }

        /// Bu fiyata KDV'nin dahil olup olmadığını belirtir.
        public bool IsTaxIncluded { get; set; } = true;

        /// SCD Type 2: Bu fiyatın geçerlilik başlangıç tarihi.
        public DateTimeOffset? ValidFrom { get; set; }

        /// SCD Type 2: Bu fiyatın bitiş tarihi. Eğer boş (Null) ise, ürünün aktif/geçerli fiyatı demektir.
        public DateTimeOffset? ValidTo { get; set; }
    }
}

