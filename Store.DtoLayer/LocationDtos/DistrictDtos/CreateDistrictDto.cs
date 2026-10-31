using System;

namespace Store.DtoLayer.LocationDtos.DistrictDtos
{
    public class CreateDistrictDto
    {
        public string DistrictName { get; set; }
        public Guid CityId { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; } = DateTime.Now;
    }
}
