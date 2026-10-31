using Store.Catalog.Entities;
using Store.Shared.Entities;
using Store.Shared.Enums;

namespace Store.Catalog.Dtos.ProductPriceDtos
{
    public class GetByIdProductPriceDto : BaseEntity
    {
        public Guid PriceTypeId { get; set; }
        public Guid? PriceListId { get; set; }
        public Guid? BasePriceId { get; set; }
        public decimal? MarginPercentage { get; set; }
        public int MinimumQuantity { get; set; } = 1;
        public decimal Amount { get; set; }
        public decimal? DiscountedAmount { get; set; }
        public bool IsTaxIncluded { get; set; } = true;
        public DateTimeOffset? ValidFrom { get; set; }
        public DateTimeOffset? ValidTo { get; set; }
        public Guid ProductId { get; set; }
        public Guid CurrencyId { get; set; }
    }
}

