using Store.Shared.Enums;

namespace Store.Catalog.Dtos.ProductDtos
{
    public class ProductVariantDto
    {
        public Guid VariantId { get; set; }
        public string Sku { get; set; } = string.Empty;

        // Renk (Referans + Snapshot)
        public Guid? ColorId { get; set; }
        public string? ColorName { get; set; }
        public string? ColorHexCode { get; set; }

        // Beden (Referans + Snapshot)
        public Guid? SizeId { get; set; }
        public string? SizeName { get; set; }
        public SizeType? SizeType { get; set; }

        public decimal ExtraPrice { get; set; }
        public int StockQuantity { get; set; }
    }
}
