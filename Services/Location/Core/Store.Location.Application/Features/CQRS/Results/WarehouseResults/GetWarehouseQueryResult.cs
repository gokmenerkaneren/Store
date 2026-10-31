namespace Store.Location.Application.Features.CQRS.Results.WarehouseResults
{
    public class GetWarehouseQueryResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public bool IsDefault { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? ContactInfoId { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid InsertUserId { get; set; }
        public DateTimeOffset InsertDate { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? DeleteUserId { get; set; }
        public DateTimeOffset? DeleteDate { get; set; }
        
        public Store.DtoLayer.LocationDtos.WarehouseDtos.WarehouseAddressDto? Address { get; set; }
    }
}
