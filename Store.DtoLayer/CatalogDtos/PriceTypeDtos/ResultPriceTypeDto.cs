namespace Store.DtoLayer.CatalogDtos.PriceTypeDtos
{
    public class ResultPriceTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string? Notes { get; set; }
        public Guid InsertUserId { get; set; }
        public DateTimeOffset InsertDate { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }
        public Guid? DeleteUserId { get; set; }
        public DateTimeOffset? DeleteDate { get; set; }
    }
}
