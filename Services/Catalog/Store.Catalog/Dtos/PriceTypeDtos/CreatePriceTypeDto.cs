using System;

namespace Store.Catalog.Dtos.PriceTypeDtos
{
    public class CreatePriceTypeDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
        public Guid InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; } = DateTime.Now;
    }
}
