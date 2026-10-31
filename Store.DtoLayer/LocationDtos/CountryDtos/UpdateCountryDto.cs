using System;

namespace Store.DtoLayer.LocationDtos.CountryDtos
{
    public class UpdateCountryDto
    {
        public Guid Id { get; set; }
        public string CountryName { get; set; }
        public string? CountryCode { get; set; }
        public string? UPhoneCode { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
    }
}
