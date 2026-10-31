using System;

namespace Store.DtoLayer.LocationDtos.WarehouseDtos
{
    public class UpdateWarehouseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public bool IsDefault { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? ContactInfoId { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
    }
}
