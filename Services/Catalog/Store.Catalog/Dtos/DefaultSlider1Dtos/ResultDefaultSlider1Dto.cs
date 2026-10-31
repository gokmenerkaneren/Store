using Store.Shared.Entities;

namespace Store.Catalog.Dtos.DefaultSlider1Dtos
{
    public class ResultDefaultSlider1Dto : BaseEntity
    {
        public int Sirano { get; set; }
        public string? ResimYolu { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string? KisaAciklama { get; set; }
    }
}
