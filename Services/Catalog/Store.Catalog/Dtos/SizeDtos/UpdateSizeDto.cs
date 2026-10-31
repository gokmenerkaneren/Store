using Store.Shared.Enums;

namespace Store.Catalog.Dtos.SizeDtos
{
    public class UpdateSizeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public SizeType SizeType { get; set; }
        public string? PrimaryValue { get; set; }
        public string? SecondaryValue { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
    }
}
