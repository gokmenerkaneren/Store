namespace Store.Location.Application.Features.CQRS.Queries.DealerQueries
{
    public class GetDealerByIdQuery
    {
        public Guid Id { get; set; }
        public GetDealerByIdQuery(Guid id) => Id = id;
    }
}
