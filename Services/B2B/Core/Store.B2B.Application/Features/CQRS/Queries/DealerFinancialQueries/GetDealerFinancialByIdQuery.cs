namespace Store.B2B.Application.Features.CQRS.Queries.DealerFinancialQueries
{
    public class GetDealerFinancialByIdQuery
    {
        public Guid Id { get; set; }

        public GetDealerFinancialByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
