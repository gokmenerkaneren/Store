using System;

namespace Store.DtoLayer.OrderDtos
{
    public class CreateOrderDetailDto
    {
        public Guid ProductId { get; set; }
        public decimal ProductPrice { get; set; } = 0;
        public int ProductAmount { get; set; } = 0;
        public decimal TotalPrice { get; set; } = 0;
        public Guid OrderingId { get; set; }
        public Guid PriceTypeId { get; set; } = Guid.Empty;
        public decimal OriginalPrice { get; set; } = 0;
        public decimal DiscountRateApplied { get; set; } = 0;
        public Guid CurrencyId { get; set; } = Guid.Empty;
        public DateTimeOffset PricingSnapshotDate { get; set; } = DateTimeOffset.Now;
    }
}

