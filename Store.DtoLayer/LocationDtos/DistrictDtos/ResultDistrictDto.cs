using System;

namespace Store.DtoLayer.LocationDtos.DistrictDtos
{
    public class ResultDistrictDto
    {
        public Guid Id { get; set; }
        public string DistrictName { get; set; }
        public Guid CityId { get; set; }
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
