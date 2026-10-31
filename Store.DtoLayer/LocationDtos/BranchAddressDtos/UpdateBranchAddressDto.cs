using System;

namespace Store.DtoLayer.LocationDtos.BranchAddressDtos
{
    public class UpdateBranchAddressDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? ContactFullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AddressDetail { get; set; }
        public Guid CityId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid NeighborhoodId { get; set; }
        public Guid BranchId { get; set; }
        public string? LoadingDockInstructions { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
    }
}
