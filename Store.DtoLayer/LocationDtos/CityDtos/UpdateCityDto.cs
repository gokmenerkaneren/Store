using System;

namespace Store.DtoLayer.LocationDtos.CityDtos
{
    public class UpdateCityDto
    {
        public Guid Id { get; set; }
        public string CityName { get; set; }
        public string? PlateCode { get; set; }
        public Guid CountryId { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
    }
}
