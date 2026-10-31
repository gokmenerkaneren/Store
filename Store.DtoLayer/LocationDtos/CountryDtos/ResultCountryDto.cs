using System;

namespace Store.DtoLayer.LocationDtos.CountryDtos
{
    public class ResultCountryDto
    {
        public Guid Id { get; set; }
        public string CountryName { get; set; }
        public string? CountryCode { get; set; }
        public string? UPhoneCode { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid InsertUserId { get; set; }
        public DateTimeOffset InsertDate { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? DeleteUserId { get; set; }
        public DateTimeOffset? DeleteDate { get; set; }
    }
}
