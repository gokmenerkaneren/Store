namespace Store.Location.Application.Features.CQRS.Queries.BranchQueries
{
    public class GetBranchByIdQuery
    {
        public Guid Id { get; set; }
        public GetBranchByIdQuery(Guid id) => Id = id;
    }
}
