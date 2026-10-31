namespace Store.Catalog.Dtos.ProductDetailDtos
{
    public class CreateProductDetailDto
    {
        public Guid ProductId { get; set; }
        public Guid BrandId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Feature { get; set; } = string.Empty;
        public string Dimension { get; set; } = string.Empty;
        public string AdditionalInfo { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; } = DateTime.Now;
    }
}

