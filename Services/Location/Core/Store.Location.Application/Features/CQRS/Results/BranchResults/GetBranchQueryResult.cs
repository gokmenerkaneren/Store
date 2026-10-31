namespace Store.Location.Application.Features.CQRS.Results.BranchResults
{
    public class GetBranchQueryResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BranchType { get; set; } = string.Empty;
        public Guid CompanyId { get; set; }
        public Guid? ContactInfoId { get; set; }
        public string? GeoLocationCoordinates { get; set; }
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
