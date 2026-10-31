namespace Store.DtoLayer.CatalogDtos.ColorDtos
{
    public class CreateColorDto
    {
        public string Name { get; set; } = string.Empty;
        public string? HexCode { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public Guid InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; } = DateTime.Now;
    }
}
