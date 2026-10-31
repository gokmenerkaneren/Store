using System;

namespace Store.DtoLayer.LocationDtos.CityDtos
{
    public class UpdateCityDto
    {
        public Guid Id { get; set; }
        public string CityName { get; set; } = string.Empty;
        public string? PlateCode { get; set; }
        public Guid CountryId { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }
}
