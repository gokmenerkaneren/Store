using Store.Shared.Entities;

namespace Store.Catalog.Dtos.ColorDtos
{
    public class ResultColorDto : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? HexCode { get; set; }
        public int DisplayOrder { get; set; }
    }
}
