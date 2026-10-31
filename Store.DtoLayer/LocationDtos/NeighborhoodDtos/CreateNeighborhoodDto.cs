using System;

namespace Store.DtoLayer.LocationDtos.NeighborhoodDtos
{
    public class CreateNeighborhoodDto
    {
        public string NeighborhoodName { get; set; }
        public string? PostalCode { get; set; }
        public Guid DistrictId { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
        public Guid InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; } = DateTime.Now;
    }
}
