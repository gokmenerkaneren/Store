using Store.DtoLayer.CommonDtos;
using Store.Location.Application.Extensions;
using Store.Location.Application.Features.CQRS.Results.DealerResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Store.Location.Application.Features.CQRS.Handlers.DealerHandlers
{
    public class GetDealersDataTablesQueryHandler
    {
        private readonly IRepository<Dealer> _repository;

        public GetDealersDataTablesQueryHandler(IRepository<Dealer> repository)
        {
            _repository = repository;
        }

        public async Task<DataTablesResponse<GetDealerQueryResult>> Handle(DataTablesRequest request, bool showDeleted = false)
        {
            var query = _repository.GetQueryable(includeDeleted: showDeleted);

            if (!showDeleted)
            {
                query = query.Where(x => !x.IsDeleted);
            }

            var mappings = new Dictionary<string, string>
            {
                { "CompanyName", "CompanyName" },
                { "TaxNumber", "TaxNumber" },
                { "TaxOffice", "TaxOffice" },
                { "DiscountRate", "DiscountRate" },
                { "CreditLimit", "CreditLimit" },
                { "CurrentBalance", "CurrentBalance" },
                { "InsertDate", "InsertDate" },
                { "UpdateDate", "UpdateDate" }
            };

            return await query.ToDataTablesResultAsync(
                request,
                mapper: x => new GetDealerQueryResult
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
                },
                customSearchFilter: (q, search) => q.Where(x => 
                    x.CompanyName.Contains(search) || 
                    (x.Notes != null && x.Notes.Contains(search))),
                columnPropertyMappings: mappings
            );
        }
    }
}
