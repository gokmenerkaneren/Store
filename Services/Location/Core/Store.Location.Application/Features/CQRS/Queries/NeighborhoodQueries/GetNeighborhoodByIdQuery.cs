namespace Store.Location.Application.Features.CQRS.Queries.NeighborhoodQueries
{
    public class GetNeighborhoodByIdQuery
    {
        public Guid Id { get; set; }
        public GetNeighborhoodByIdQuery(Guid id) => Id = id;
    }
}
