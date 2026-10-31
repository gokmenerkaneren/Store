using System;

namespace Store.Catalog.Dtos.PricingDtos
{
    public class CalculatedPriceDto
    {
        public Guid ProductId { get; set; }
        public decimal BasePrice { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal DiscountRateApplied { get; set; }
        public decimal DiscountAmount { get; set; }
        public Guid CurrencyId { get; set; }
        public Guid PriceTypeId { get; set; }
        public string PriceTypeName { get; set; } = string.Empty;
        public bool IsTaxIncluded { get; set; }
    }
}
