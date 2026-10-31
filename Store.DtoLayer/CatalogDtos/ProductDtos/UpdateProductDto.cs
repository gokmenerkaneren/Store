using MediatR;
using Store.DtoLayer.CatalogDtos.MeasurementDtos;
using System.Collections.Generic;

namespace Store.DtoLayer.CatalogDtos.ProductDtos
{
    public class UpdateProductDto : IRequest<bool>
    {
        public Guid Id { get; set; }
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
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
    }
}
