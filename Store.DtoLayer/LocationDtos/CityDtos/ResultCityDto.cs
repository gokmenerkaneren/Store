using System;

namespace Store.DtoLayer.LocationDtos.CityDtos
{
    public class ResultCityDto
    {
        public Guid Id { get; set; }
        public string CityName { get; set; }
        public string? PlateCode { get; set; }
        public Guid CountryId { get; set; }
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
