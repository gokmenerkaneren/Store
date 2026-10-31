using Store.Catalog.Entities;
using Store.Shared.Entities;
using Store.Shared.Enums;

namespace Store.Catalog.Dtos.ProductDetailDtos
{
    public class GetByIdProductDetailDto : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Guid BrandId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Feature { get; set; } = string.Empty;
        public string Dimension { get; set; } = string.Empty;
        public string AdditionalInfo { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
    }
}

