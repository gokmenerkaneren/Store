using Store.Shared.Entities;
using Store.Shared.Enums;

namespace Store.Catalog.Entities
{
    // =========================================================================
    // SIZE (Beden Tanım Tablosu)
    // Sistemdeki tüm beden seçeneklerini merkezi olarak tutar.
    // SizeType enum'u ile beden sistemi (harf, sayı, denim, ayakkabı) ayrıştırılır.
    // ProductVariant içinde SizeId ile referans edilir.
    // =========================================================================
    public class Size : BaseEntity
    {
        /// <summary>
        /// Bedenin kullanıcıya gösterilecek tam adı.
        /// Örnekler:
        ///   Clothing → "S", "M", "L", "XL"
        ///   Number   → "36", "38", "40"
        ///   Denim    → "34/31" (bel/boy birleşimi)
        ///   Shoe     → "39", "40", "41"
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Bedenin ait olduğu ölçüm sistemi.
        /// Aynı koleksiyonda farklı tipteki bedenlerin birlikte saklanmasına olanak tanır.
        /// Bkz: Store.Shared.Enums.SizeType
        /// </summary>
        public SizeType SizeType { get; set; } = SizeType.Clothing;

        /// <summary>
        /// Ana beden değeri.
        /// Clothing → "M", Number → "38", Denim → "34" (bel), Shoe → "40"
        /// </summary>
        public string? PrimaryValue { get; set; }

        /// <summary>
        /// İkinci beden değeri. Yalnızca Denim tipinde kullanılır.
        /// Denim → "31" (bacak boyu). Diğer tipler için null bırakılır.
        /// </summary>
        public string? SecondaryValue { get; set; }

        /// <summary>
        /// Listeleme ekranlarında sıralama için kullanılır.
        /// Küçük sayı → önce gösterilir.
        /// </summary>
        public int DisplayOrder { get; set; } = 0;
    }
}
