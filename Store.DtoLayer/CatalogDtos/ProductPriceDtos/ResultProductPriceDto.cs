namespace Store.DtoLayer.CatalogDtos.ProductPriceDtos
{
    public class ResultProductPriceDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid PriceTypeId { get; set; }
        public decimal Amount { get; set; }
        public decimal? DiscountedAmount { get; set; }
        public Guid CurrencyId { get; set; }
        public bool IsTaxIncluded { get; set; } = true;
        public Guid? PriceListId { get; set; }
        public int MinimumQuantity { get; set; }
        public DateTimeOffset? ValidFrom { get; set; }
        public DateTimeOffset? ValidTo { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid InsertUserId { get; set; }
        public DateTimeOffset InsertDate { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? DeleteUserId { get; set; }
        public DateTimeOffset? DeleteDate { get; set; }
    }
}

