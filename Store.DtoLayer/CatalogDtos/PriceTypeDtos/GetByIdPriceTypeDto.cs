namespace Store.DtoLayer.CatalogDtos.PriceTypeDtos
{
    public class GetByIdPriceTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }
}
