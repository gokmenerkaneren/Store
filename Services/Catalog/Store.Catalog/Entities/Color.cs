using Store.Shared.Entities;

namespace Store.Catalog.Entities
{
    // =========================================================================
    // COLOR (Renk Tanım Tablosu)
    // Sistemdeki tüm renk seçeneklerini merkezi olarak tutar.
    // ProductVariant içinde ColorId ile referans edilir.
    // =========================================================================
    public class Color : BaseEntity
    {
        /// <summary>
        /// Rengin kullanıcıya gösterilecek adı. Örn: "Kırmızı", "Petrol Mavisi"
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Rengin HTML/CSS hex kodu. Örn: "#FF0000", "#008080"
        /// Sidebar renk dairelerinde inline style ile kullanılır: background-color: #008080
        /// Admin panelinde input[type=color] ile seçilir ve otomatik dolar.
        /// </summary>
        public string? HexCode { get; set; }

        /// <summary>
        /// Sidebar ve listeleme ekranlarında sıralama için kullanılır.
        /// Küçük sayı → önce gösterilir.
        /// </summary>
        public int DisplayOrder { get; set; } = 0;
    }
}
