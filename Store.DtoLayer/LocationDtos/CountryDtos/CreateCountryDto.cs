using System;

namespace Store.DtoLayer.LocationDtos.CountryDtos
{
    public class CreateCountryDto
    {
        public string CountryName { get; set; }
        public string? CountryCode { get; set; }
        public string? UPhoneCode { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
        public Guid InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; } = DateTime.Now;
    }
}
