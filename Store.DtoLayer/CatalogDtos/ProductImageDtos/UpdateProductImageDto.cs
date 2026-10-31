namespace Store.DtoLayer.CatalogDtos.ProductImageDtos
{
    public class UpdateProductImageDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }
        public bool IsMain { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
    }
}
