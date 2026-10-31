using Store.Location.Application.Features.CQRS.Queries.CompanyQueries;
using Store.Shared.Enums;
using Store.Location.Application.Features.CQRS.Results.CompanyResults;
using Store.Shared.Enums;
using Store.Location.Application.Interfaces;
using Store.Shared.Enums;
using Store.Location.Domain.Entities;
using Store.Shared.Enums;

namespace Store.Location.Application.Features.CQRS.Handlers.CompanyHandlers
{
    public class GetCompanyByIdQueryHandler
    {
        private readonly IRepository<Company> _repository;

        public GetCompanyByIdQueryHandler(IRepository<Company> repository)
        {
            _repository = repository;
        }

        public async Task<GetCompanyByIdQueryResult?> Handle(GetCompanyByIdQuery query)
        {
            var values = await _repository.GetByFilterAsync(x => x.Id == query.Id);
            if (values == null) return null;

            return new GetCompanyByIdQueryResult
            {
                Id = values.Id,
                LegalName = values.LegalName,
                BrandName = values.BrandName,
                TaxOffice = values.TaxOffice,
                TaxNumber = values.TaxNumber,
                TradeRegistryNumber = values.TradeRegistryNumber,
                MersisNumber = values.MersisNumber,
                Website = values.Website,
                LogoUrl = values.LogoUrl,
                MainContactName = values.MainContactName,
                Email = values.Email,
                Phone = values.Phone,
                IsActive = values.IsActive,
                IsDeleted = values.IsDeleted,
                Notes = values.Notes,
                InvoiceType = values.InvoiceType,
                IsEInvoiceUser = values.IsEInvoiceUser,
                ErpCode = values.ErpCode ?? string.Empty,
                CountryId = values.CountryId,
                BillingAddressId = values.BillingAddressId,
                InsertDate = values.InsertDate,
                UpdateDate = values.UpdateDate,
                InsertUserId = values.InsertUserId,
                UpdateUserId = values.UpdateUserId,
                DeleteUserId = values.DeleteUserId,
                DeleteDate = values.DeleteDate,
            };
        }
    }
}

