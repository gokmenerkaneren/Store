using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Store.Shared.Entities;
using Store.Shared.Enums;

namespace Store.Catalog.Entities
{
    // =========================================================================
    // 15. PRODUCT (Ana Ürün Kartı)
    // Ürüne ait temel, mali, lojistik ve SEO bilgilerini tutar.
    // Fiyat, Stok, Resim ve Dinamik Özellikler alt tablolarda tutulur.
    // =========================================================================
    public class Product : BaseEntity
    {
        // --- TEMEL BİLGİLER ---
        public string ProductCode { get; set; } = string.Empty; // Şirketin kullandığı Stok/SKU kodu (Örn: DAFT017-MT02).
        public string? Barcode { get; set; } // Ürünün EAN/UPC barkod numarası. El terminali ile depo sayımı ve çıkışı için kritik.
        public string Name { get; set; } = string.Empty; // Sitede görünecek ürün adı (Örn: Alice Arka Fon).
        public string? ShortDescription { get; set; } // Kategori listeleme sayfalarında ürünün altında çıkacak 1-2 cümlelik özet.
        public string? Description { get; set; } // Ürün detay sayfasındaki zengin metin (HTML) detaylı açıklama.

        // --- SATIŞ, KİRALAMA VE GÖRÜNÜM DURUMLARI ---
        public bool Satis { get; set; } = true;
        public bool Kiralama { get; set; } = true;
        public bool IsPublished { get; set; } = true; // Ürün yayında mı? (Taslak olarak kaydedilip henüz sitede gösterilmemesi için).

        // --- MALİ VE MUHASEBE BİLGİLERİ ---
        public int TaxRate { get; set; } = 20; // Ürünün KDV oranı (Örn: 20, 10, 1). Sepet ve fatura hesaplamalarında zorunludur.

        // --- LOJİSTİK VE KARGO (Fiziksel Sabitler) ---
        public double? Weight { get; set; } // Ürünün ağırlığı (Kg cinsinden). Kargo fiyatlandırması için.
        public double? Desi { get; set; } // Ürünün hacimsel ağırlığı (Desi). Nakliye/Kargo şirketlerinin fiyat hesaplaması için.
        public double? Width { get; set; } // Ürünün Genişliği
        public double? Height { get; set; } // Ürünün Yüksekliği
        public double? Depth { get; set; } // Ürünün Derinliği
        // --- SEO (ARAMA MOTORU OPTİMİZASYONU) ---
        public string Slug { get; set; } = string.Empty; // Ürünün temiz URL yolu (Örn: alice-arka-fon). Link yapısı için zorunlu.
        public string? MetaTitle { get; set; } // Google arama sonuçlarında (sekmede) görünecek SEO başlığı.
        public string? MetaDescription { get; set; } // Google arama sonuçlarında ürünün altında çıkacak açıklama metni.

        // --- REFERANSLAR (RELATIONS / FOREIGN KEYS) ---

        public Guid CategoryId { get; set; } // Ürünün bağlı olduğu Kategori ID'si (Zorunlu).

        public Guid? BrandId { get; set; } // Ürünün bağlı olduğu Marka ID'si (Kendi üretiminiz olan veya markasız ürünler için Nullable/Soru işaretli yapıldı).

        public Guid MeasurementId { get; set; } // Ürünün hangi birimle (Adet, Metre, Takım) işlem gördüğünü tutan Measurement (Birim) tablosunun ID'si.

        // --- GÖMÜLÜ DOKÜMANLAR (EMBEDDED DOCUMENTS) ---
        public List<ProductAttribute> Attributes { get; set; } = new();
        public List<ProductVariant> Variants { get; set; } = new();
        public List<ProductPrice> ActivePrices { get; set; } = new();
        public List<string> Tags { get; set; } = new();
    }
}

