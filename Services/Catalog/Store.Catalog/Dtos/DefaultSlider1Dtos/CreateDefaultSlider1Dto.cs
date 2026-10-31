namespace Store.Catalog.Dtos.DefaultSlider1Dtos
{
    public class CreateDefaultSlider1Dto
    {
        public int Sirano { get; set; }
        public string? ResimYolu { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string? KisaAciklama { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; } = DateTime.Now;
    }
}
