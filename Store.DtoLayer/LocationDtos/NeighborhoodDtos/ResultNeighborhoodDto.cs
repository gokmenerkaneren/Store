using System;

namespace Store.DtoLayer.LocationDtos.NeighborhoodDtos
{
    public class ResultNeighborhoodDto
    {
        public Guid Id { get; set; }
        public string NeighborhoodName { get; set; }
        public string? PostalCode { get; set; }
        public Guid DistrictId { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid InsertUserId { get; set; }
        public DateTimeOffset InsertDate { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? DeleteUserId { get; set; }
        public DateTimeOffset? DeleteDate { get; set; }
    }
}
