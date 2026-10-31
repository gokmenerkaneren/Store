using Store.Shared.Entities;

namespace Store.Catalog.Entities
{
    /// <summary>
    /// Ana Ekran Slider kaydi. Kullanici arayuzunde gosterilecek slider/banner
    /// ogelerinin bilgilerini (sira, resim, baslik, kisa aciklama) tutar.
    /// Tum audit alanlari (IsActive, IsDeleted, InsertDate vb.) BaseEntity'den miras alinir.
    /// </summary>
    public class DefaultSlider1 : BaseEntity
    {
        /// <summary>Slider ogesinin liste icindeki gosterim sirasi.</summary>
        public int Sirano { get; set; }

        /// <summary>Yuklenen slider gorselinin sunucudaki yolu (opsiyonel, null olabilir).</summary>
        public string? ResimYolu { get; set; }

        /// <summary>Slider uzerinde gosterilecek ana baslik metni.</summary>
        public string Baslik { get; set; } = string.Empty;

        /// <summary>Slider altinda gosterilecek kisa aciklama/alt baslik metni (opsiyonel).</summary>
        public string? KisaAciklama { get; set; }
    }
}
