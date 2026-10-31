namespace Store.Catalog.Dtos.ProductDtos
{
    using System;
    using System.Collections.Generic;

    public class CreateProductDto
    {
        public string ProductCode { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public bool Satis { get; set; } = true;
        public bool Kiralama { get; set; } = true;
        public bool IsPublished { get; set; } = true;
        public int TaxRate { get; set; } = 20;
        public double? Weight { get; set; }
        public double? Desi { get; set; }
        public double? Width { get; set; }
        public double? Height { get; set; }
        public double? Depth { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? BrandId { get; set; }
        public Guid MeasurementId { get; set; }
        public List<ProductAttributeDto> Attributes { get; set; } = new();
        public List<ProductVariantDto> Variants { get; set; } = new();
        public List<string> Tags { get; set; } = new();
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; } = DateTime.Now;
    }
}

