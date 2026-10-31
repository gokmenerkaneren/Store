using Store.Shared.Entities;
using Store.Shared.Enums;

namespace Store.Catalog.Dtos.SizeDtos
{
    public class GetByIdSizeDto : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public SizeType SizeType { get; set; }
        public string? PrimaryValue { get; set; }
        public string? SecondaryValue { get; set; }
        public int DisplayOrder { get; set; }
    }
}
