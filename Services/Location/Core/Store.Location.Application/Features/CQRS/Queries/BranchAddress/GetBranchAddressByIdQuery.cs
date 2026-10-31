namespace Store.Location.Application.Features.CQRS.Queries.BranchAddress
{
    public class GetBranchAddressByIdQuery
    {
        public Guid Id { get; set; }
        public GetBranchAddressByIdQuery(Guid id) => Id = id;
    }
}
