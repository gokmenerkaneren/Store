namespace Store.Location.Application.Features.CQRS.Queries.DealerAddressQueries
{
    public class GetDealerAddressByIdQuery
    {
        public Guid Id { get; set; }
        public GetDealerAddressByIdQuery(Guid id) => Id = id;
    }

    public class GetDealerAddressesByDealerIdQuery
    {
        public Guid DealerId { get; set; }
        public GetDealerAddressesByDealerIdQuery(Guid dealerId) => DealerId = dealerId;
    }
}
