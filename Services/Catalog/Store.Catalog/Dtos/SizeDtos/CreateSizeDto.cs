using Store.Shared.Enums;

namespace Store.Catalog.Dtos.SizeDtos
{
    public class CreateSizeDto
    {
        public string Name { get; set; } = string.Empty;         // "M", "38", "34/31"
        public SizeType SizeType { get; set; } = SizeType.Clothing;
        public string? PrimaryValue { get; set; }                // "M", "38", "34"
        public string? SecondaryValue { get; set; }              // Sadece Denim → "31"
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public Guid InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; } = DateTime.Now;
    }
}
