using System;

namespace Store.DtoLayer.CatalogDtos.ProductDtos
{
    public class ProductVariantDto
    {
        public Guid VariantId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public decimal ExtraPrice { get; set; }
        public int StockQuantity { get; set; }
    }
}
