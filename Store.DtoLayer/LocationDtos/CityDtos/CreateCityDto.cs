using System;

namespace Store.DtoLayer.LocationDtos.CityDtos
{
    public class CreateCityDto
    {
        public string CityName { get; set; }
        public string? PlateCode { get; set; }
        public Guid CountryId { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; } = DateTime.Now;
    }
}
