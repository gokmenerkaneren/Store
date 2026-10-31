using System;

namespace Store.DtoLayer.LocationDtos.WarehouseDtos
{
    public class CreateWarehouseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public bool IsDefault { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? ContactInfoId { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
        public Guid InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; } = DateTime.Now;
    }
}
