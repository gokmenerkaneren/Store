namespace Store.DtoLayer.CatalogDtos.MeasurementDtos
{
    public class CreateMeasurementDto
    {
        public string Code { get; set; } = string.Empty; 
        public string Name { get; set; } = string.Empty; 
        public string Symbol { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
        public Guid InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; } = DateTime.Now;
    }
}
