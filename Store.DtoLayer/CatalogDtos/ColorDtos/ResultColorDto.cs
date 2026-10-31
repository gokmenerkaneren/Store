namespace Store.DtoLayer.CatalogDtos.ColorDtos
{
    public class ResultColorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// HTML hex renk kodu. Sidebar'da inline style ile kullanılır.
        /// Örn: "#FF0000" → style="background-color: #FF0000"
        /// </summary>
        public string? HexCode { get; set; }

        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }   // Silinmiş kayıt kontrolü için
        public DateTimeOffset InsertDate { get; set; }
    }
}
