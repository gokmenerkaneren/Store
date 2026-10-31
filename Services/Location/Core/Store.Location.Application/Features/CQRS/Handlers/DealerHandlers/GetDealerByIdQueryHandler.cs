using Store.Location.Application.Features.CQRS.Queries.DealerQueries;
using Store.Location.Application.Features.CQRS.Results.DealerResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.DealerHandlers
{
    public class GetDealerByIdQueryHandler
    {
        private readonly IRepository<Dealer> _repository;

        public GetDealerByIdQueryHandler(IRepository<Dealer> repository)
        {
            _repository = repository;
        }

        public async Task<GetDealerByIdQueryResult?> Handle(GetDealerByIdQuery query)
        {
            var values = await _repository.GetByFilterAsync(x => x.Id == query.Id);
            if (values == null) return null;

            return new GetDealerByIdQueryResult
            {
                Id = values.Id,
                CompanyName = values.CompanyName,

                ContactInfoId = values.ContactInfoId,
                IsActive = values.IsActive,
                IsDeleted = values.IsDeleted,
                Notes = values.Notes,
                InsertDate = values.InsertDate,
                UpdateDate = values.UpdateDate,
                InsertUserId = values.InsertUserId,
                UpdateUserId = values.UpdateUserId,
                DeleteUserId = values.DeleteUserId,
                DeleteDate = values.DeleteDate,
                TaxNumber = values.TaxNumber,
                TaxOffice = values.TaxOffice,
                DiscountRate = values.DiscountRate,
                CreditLimit = values.CreditLimit,
                CurrentBalance = values.CurrentBalance,
            };
        }
    }
}

