using Store.B2B.Application.Features.CQRS.Queries.DealerFinancialQueries;
using Store.B2B.Application.Features.CQRS.Results.DealerFinancialResults;
using Store.B2B.Application.Interfaces;
using Store.B2B.Domain.Entities;

namespace Store.B2B.Application.Features.CQRS.Handlers.DealerFinancialHandlers
{
    public class GetDealerFinancialByIdQueryHandler
    {
        private readonly IB2BRepository<DealerFinancial> _repository;

        public GetDealerFinancialByIdQueryHandler(IB2BRepository<DealerFinancial> repository)
        {
            _repository = repository;
        }

        public async Task<GetDealerFinancialByIdQueryResult?> Handle(GetDealerFinancialByIdQuery query)
        {
            var value = await _repository.GetByIdAsync(query.Id);
            if (value is null) return null;

            return new GetDealerFinancialByIdQueryResult
            {
                Id = value.Id,
                DealerId = value.DealerId,
                TaxNumber = value.TaxNumber,
                TaxOffice = value.TaxOffice,
                DiscountRate = value.DiscountRate,
                CreditLimit = value.CreditLimit,
                CurrentBalance = value.CurrentBalance,
                CreatedDate = value.InsertDate.DateTime,
                UpdatedDate = value.UpdateDate?.DateTime
            };
        }
    }
}
