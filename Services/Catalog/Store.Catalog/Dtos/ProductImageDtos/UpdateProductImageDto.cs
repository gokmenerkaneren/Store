namespace Store.Catalog.Dtos.ProductImageDtos
{
    public class UpdateProductImageDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public int DisplayOrder { get; set; }
        public string AltText { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
    }
}

