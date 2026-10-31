namespace Store.Location.Application.Features.CQRS.Queries.UserAddressQueries
{
    public class GetUserAddressByIdQuery
    {
        public Guid Id { get; set; }
        public GetUserAddressByIdQuery(Guid id) => Id = id;
    }
}
