using System;

namespace Store.DtoLayer.LocationDtos.NeighborhoodDtos
{
    public class UpdateNeighborhoodDto
    {
        public Guid Id { get; set; }
        public string NeighborhoodName { get; set; }
        public string? PostalCode { get; set; }
        public Guid DistrictId { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
    }
}
