using System;

namespace Store.Catalog.Dtos.ProductPriceDtos
{
    public class BulkPriceUpdateDto
    {
        public Guid SourcePriceTypeId { get; set; }
        public Guid TargetPriceTypeId { get; set; }
        public decimal MarginPercentage { get; set; }
        
        // Audit (Denetim) Alanı - Arka plan görevlerinde HttpContext olmadığı için zorunlu
        public Guid AdminUserId { get; set; }
        
        // Opsiyonel Filtreler
        public Guid? CategoryId { get; set; }
        public Guid? BrandId { get; set; }
    }
}
