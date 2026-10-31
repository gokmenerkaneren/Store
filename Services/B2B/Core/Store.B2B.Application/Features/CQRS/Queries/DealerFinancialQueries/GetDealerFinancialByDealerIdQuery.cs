namespace Store.B2B.Application.Features.CQRS.Queries.DealerFinancialQueries
{
    public class GetDealerFinancialByDealerIdQuery
    {
        public Guid DealerId { get; set; }

        public GetDealerFinancialByDealerIdQuery(Guid dealerId)
        {
            DealerId = dealerId;
        }
    }
}
