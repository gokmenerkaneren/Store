using System;

namespace Store.DtoLayer.LocationDtos.BranchDtos
{
    public class UpdateBranchDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? BranchType { get; set; }
        public Guid CompanyId { get; set; }
        public Guid? ContactInfoId { get; set; }
        public string? GeoLocationCoordinates { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
    }
}
