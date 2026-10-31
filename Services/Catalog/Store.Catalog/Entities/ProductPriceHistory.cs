using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Store.Shared.Entities;

namespace Store.Catalog.Entities
{
    /// <summary>
    /// Süresi dolan veya değişen geçmiş fiyatların arşivlendiği koleksiyon.
    /// Product dokümanının sürekli büyümesini (Unbounded Growth) engeller.
    /// </summary>
    public class ProductPriceHistory : BaseEntity
    {
        [BsonRepresentation(BsonType.String)]
        public Guid ProductId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid CurrencyId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid PriceTypeId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid? PriceListId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid? BasePriceId { get; set; }

        public decimal? MarginPercentage { get; set; }

        public int MinimumQuantity { get; set; }
        public decimal Amount { get; set; }
        public decimal? DiscountedAmount { get; set; }
        public bool IsTaxIncluded { get; set; }
        public DateTimeOffset? ValidFrom { get; set; }
        public DateTimeOffset? ValidTo { get; set; }
        public DateTimeOffset ArchivedDate { get; set; } = DateTimeOffset.UtcNow;
    }
}
