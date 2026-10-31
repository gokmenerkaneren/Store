namespace Store.Catalog.Dtos.DefaultSlider1Dtos
{
    public class UpdateDefaultSlider1Dto
    {
        public Guid Id { get; set; }
        public int Sirano { get; set; }
        public string? ResimYolu { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string? KisaAciklama { get; set; }
        public bool IsActive { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
    }
}
