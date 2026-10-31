using System;

namespace Store.DtoLayer.CatalogDtos.DefaultSlider1Dtos
{
    public class ResultDefaultSlider1Dto
    {
        public Guid Id { get; set; }
        public int Sirano { get; set; }
        public string? ResimYolu { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string? KisaAciklama { get; set; }
        public bool IsActive { get; set; }
        public Guid InsertUserId { get; set; }
        public DateTimeOffset InsertDate { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? DeleteUserId { get; set; }
        public DateTimeOffset? DeleteDate { get; set; }
    }
}
