namespace Store.DtoLayer.CatalogDtos.PriceTypeDtos
{
    public class UpdatePriceTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
