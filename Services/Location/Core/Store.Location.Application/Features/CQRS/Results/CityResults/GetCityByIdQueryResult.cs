namespace Store.Location.Application.Features.CQRS.Results.CityResults
{
    public class GetCityByIdQueryResult
    {
        public Guid Id { get; set; }
        public string CityName { get; set; } = string.Empty;
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
