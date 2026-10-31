namespace Store.Catalog.Dtos.ProductPriceDtos
{
    public class UpdateProductPriceDto
    {
        public Guid Id { get; set; }
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
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
    }
}

