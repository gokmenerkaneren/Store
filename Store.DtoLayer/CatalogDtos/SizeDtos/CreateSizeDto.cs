namespace Store.DtoLayer.CatalogDtos.SizeDtos
{
    public class CreateSizeDto
    {
        public string Name { get; set; } = string.Empty;
        public int SizeType { get; set; }           // SizeType enum int değeri: 1=Clothing, 2=Number, 3=Denim, 4=Shoe
        public string? PrimaryValue { get; set; }
        public string? SecondaryValue { get; set; } // Yalnızca Denim için bacak boyu
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public Guid InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; } = DateTime.Now;
    }
}
