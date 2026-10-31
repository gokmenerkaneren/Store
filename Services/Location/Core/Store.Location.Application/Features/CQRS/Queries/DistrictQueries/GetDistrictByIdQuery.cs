namespace Store.Location.Application.Features.CQRS.Queries.DistrictQueries
{
    public class GetDistrictByIdQuery
    {
        public Guid Id { get; set; }
        public GetDistrictByIdQuery(Guid id) => Id = id;
    }
}
