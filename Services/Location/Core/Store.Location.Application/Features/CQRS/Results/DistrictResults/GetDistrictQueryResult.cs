namespace Store.Location.Application.Features.CQRS.Results.DistrictResults
{
    public class GetDistrictQueryResult
    {
        public Guid Id { get; set; }
        public string DistrictName { get; set; } = string.Empty;
        public Guid CityId { get; set; }
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
