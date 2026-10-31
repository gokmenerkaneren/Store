using System;

namespace Store.DtoLayer.LocationDtos.WarehouseDtos
{
    public class WarehouseAddressDto
    {
        public Guid? Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ContactFullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AddressDetail { get; set; } = string.Empty;
        public Guid CountryId { get; set; }
        public Guid CityId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid? NeighborhoodId { get; set; }
        public string? LoadingDockInstructions { get; set; }
    }
}
