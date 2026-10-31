namespace Store.Catalog.Dtos.ColorDtos
{
    public class CreateColorDto
    {
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// HTML hex renk kodu. Admin panelinde input[type=color] ile otomatik dolar.
        /// Örn: "#FF0000", "#008080"
        /// </summary>
        public string? HexCode { get; set; }

        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public Guid InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; } = DateTime.Now;
    }
}
