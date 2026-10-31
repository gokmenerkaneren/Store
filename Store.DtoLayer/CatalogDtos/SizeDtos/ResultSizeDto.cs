namespace Store.DtoLayer.CatalogDtos.SizeDtos
{
    public class ResultSizeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;      // "M", "38", "34/31"
        public int SizeType { get; set; }                     // SizeType enum değeri (int)
        public string? SizeTypeName { get; set; }             // "Clothing", "Denim" vb. (display)
        public string? PrimaryValue { get; set; }
        public string? SecondaryValue { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }   // Silinmiş kayıt kontrolü için
        public DateTimeOffset InsertDate { get; set; }
    }
}
