using Store.Location.Application.Features.CQRS.Queries.DealerQueries;
using Store.Location.Application.Features.CQRS.Results.DealerResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.DealerHandlers
{
    public class GetDealersQueryHandler
    {
        private readonly IRepository<Dealer> _repository;

        public GetDealersQueryHandler(IRepository<Dealer> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetDealerQueryResult>> Handle(bool showDeleted = false)
        {
            var dealers = await _repository.GetAllAsync(includeDeleted: showDeleted);
            if (!showDeleted) dealers = dealers.Where(x => !x.IsDeleted).ToList();
            return dealers.Select(x => new GetDealerQueryResult
            {
                Id = x.Id,
                CompanyName = x.CompanyName,

                ContactInfoId = x.ContactInfoId,
                IsActive = x.IsActive,
                Notes = x.Notes,
                InsertUserId = x.InsertUserId,
                InsertDate = x.InsertDate,
                UpdateUserId = x.UpdateUserId,
                UpdateDate = x.UpdateDate,
                IsDeleted = x.IsDeleted,
                DeleteUserId = x.DeleteUserId,
                DeleteDate = x.DeleteDate,
                TaxNumber = x.TaxNumber,
                TaxOffice = x.TaxOffice,
                DiscountRate = x.DiscountRate,
                CreditLimit = x.CreditLimit,
                CurrentBalance = x.CurrentBalance,
            }).ToList();
        }
    }
}
