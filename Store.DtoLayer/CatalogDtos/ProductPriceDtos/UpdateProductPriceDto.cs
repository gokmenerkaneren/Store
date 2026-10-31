namespace Store.DtoLayer.CatalogDtos.ProductPriceDtos
{
    public class UpdateProductPriceDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid PriceTypeId { get; set; }
        public decimal Amount { get; set; }
        public decimal? DiscountedAmount { get; set; }
        public bool IsTaxIncluded { get; set; } = true;
        public Guid CurrencyId { get; set; }
        public Guid? PriceListId { get; set; }
        public int MinimumQuantity { get; set; } = 1;
        public DateTimeOffset? ValidFrom { get; set; }
        public DateTimeOffset? ValidTo { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
    }
}

