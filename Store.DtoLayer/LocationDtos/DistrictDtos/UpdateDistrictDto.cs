using System;

namespace Store.DtoLayer.LocationDtos.DistrictDtos
{
    public class UpdateDistrictDto
    {
        public Guid Id { get; set; }
        public string DistrictName { get; set; }
        public Guid CityId { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
    }
}
