using System;

namespace Store.DtoLayer.LocationDtos.BranchDtos
{
    public class CreateBranchDto
    {
        public string Name { get; set; }
        public string BranchType { get; set; }
        public Guid CompanyId { get; set; }
        public Guid? ContactInfoId { get; set; }
        public string? GeoLocationCoordinates { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
        public Guid InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; } = DateTime.Now;
    }
}
