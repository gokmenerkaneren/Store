using Store.Catalog.Entities;
using Store.Shared.Entities;
using Store.Shared.Enums;

namespace Store.Catalog.Dtos.ProductImageDtos
{
    public class GetByIdProductImageDto : BaseEntity
    {
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public int DisplayOrder { get; set; }
        public string AltText { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
    }
}

