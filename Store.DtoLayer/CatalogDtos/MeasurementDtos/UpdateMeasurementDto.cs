namespace Store.DtoLayer.CatalogDtos.MeasurementDtos
{
    public class UpdateMeasurementDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty; 
        public string Name { get; set; } = string.Empty; 
        public string Symbol { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
    }
}
