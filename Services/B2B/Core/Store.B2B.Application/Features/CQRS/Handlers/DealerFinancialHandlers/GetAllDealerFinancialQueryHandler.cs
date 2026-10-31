using Store.B2B.Application.Features.CQRS.Queries.DealerFinancialQueries;
using Store.B2B.Application.Features.CQRS.Results.DealerFinancialResults;
using Store.B2B.Application.Interfaces;
using Store.B2B.Domain.Entities;

namespace Store.B2B.Application.Features.CQRS.Handlers.DealerFinancialHandlers
{
    public class GetAllDealerFinancialQueryHandler
    {
        private readonly IB2BRepository<DealerFinancial> _repository;

        public GetAllDealerFinancialQueryHandler(IB2BRepository<DealerFinancial> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetAllDealerFinancialQueryResult>> Handle(GetAllDealerFinancialQuery query)
        {
            var values = await _repository.GetAllAsync();
            return values.Select(x => new GetAllDealerFinancialQueryResult
            {
                Id = x.Id,
                DealerId = x.DealerId,
                TaxNumber = x.TaxNumber,
                TaxOffice = x.TaxOffice,
                DiscountRate = x.DiscountRate,
                CreditLimit = x.CreditLimit,
                CurrentBalance = x.CurrentBalance,
                CreatedDate = x.InsertDate.DateTime
            }).ToList();
        }
    }
}
