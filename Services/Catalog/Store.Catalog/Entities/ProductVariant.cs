using Store.Shared.Enums;

namespace Store.Catalog.Entities
{
    // =========================================================================
    // PRODUCT VARIANT (Ürün Varyantı)
    // Bir ürünün farklı renk/beden kombinasyonlarını ve bunlara ait
    // stok/fiyat farklılıklarını tutar.
    // Color ve Size, lookup tablolarına (Colors, Sizes koleksiyonları) ID ile bağlıdır.
    // Performans için ColorName, HexCode ve SizeName snapshot olarak saklanır.
    // =========================================================================
    public class ProductVariant
    {
        public Guid VariantId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Bu varyanta özel stok takip kodu. Örn: "DAFT017-MT02-KIRMIZI-M"
        /// </summary>
        public string Sku { get; set; } = string.Empty;

        // --- RENK (COLOR) ---
        /// <summary>
        /// Colors koleksiyonundaki Color kaydının ID'si. (Referans)
        /// </summary>
        public Guid? ColorId { get; set; }

        /// <summary>
        /// [SNAPSHOT] Rengin görüntüleme adı. Colors koleksiyonundan kopyalanır.
        /// Tekrarlı sorgu maliyetini önlemek için burada da saklanır.
        /// Örn: "Kırmızı", "Petrol Mavisi"
        /// </summary>
        public string? ColorName { get; set; }

        /// <summary>
        /// [SNAPSHOT] Rengin hex kodu. Sidebar renk dairelerinde kullanılır.
        /// Örn: "#FF0000" → style="background-color: #FF0000"
        /// </summary>
        public string? ColorHexCode { get; set; }

        // --- BEDEN (SIZE) ---
        /// <summary>
        /// Sizes koleksiyonundaki Size kaydının ID'si. (Referans)
        /// </summary>
        public Guid? SizeId { get; set; }

        /// <summary>
        /// [SNAPSHOT] Bedenin görüntüleme adı. Sizes koleksiyonundan kopyalanır.
        /// Örn: "M", "38", "34/31"
        /// </summary>
        public string? SizeName { get; set; }

        /// <summary>
        /// [SNAPSHOT] Bedenin tipi. Sidebar ve filtre gruplandırması için kullanılır.
        /// Örn: Clothing, Denim, Shoe...
        /// </summary>
        public SizeType? SizeType { get; set; }

        // --- FİYAT VE STOK ---
        /// <summary>
        /// Bu varyantın ana ürün fiyatına göre ek fiyatı (artı ya da eksi olabilir).
        /// Örn: XL beden 10₺ daha pahalıysa ExtraPrice = 10.
        /// </summary>
        public decimal ExtraPrice { get; set; } = 0;

        /// <summary>
        /// Bu renk/beden kombinasyonuna ait stok adedi.
        /// </summary>
        public int StockQuantity { get; set; } = 0;
    }
}
