using Store.Location.Application.Features.CQRS.Results.CompanyResults;
using Store.Shared.Enums;
using Store.Location.Application.Interfaces;
using Store.Shared.Enums;
using Store.Location.Domain.Entities;
using Store.Shared.Enums;
using System;
using Store.Shared.Enums;
using System.Collections.Generic;
using Store.Shared.Enums;
using System.Linq;
using Store.Shared.Enums;
using System.Threading.Tasks;
using Store.Shared.Enums;

namespace Store.Location.Application.Features.CQRS.Handlers
{
    public class GetCompanyQueryHandler
    {
        private readonly IRepository<Company> _repository;

        public GetCompanyQueryHandler(IRepository<Company> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetCompanyByIdQueryResult>> Handle(bool showDeleted = false)
        {
            var companies = await _repository.GetAllAsync(includeDeleted: showDeleted);
            if (!showDeleted) companies = companies.Where(x => !x.IsDeleted).ToList();
            return companies.Select(x => new GetCompanyByIdQueryResult
            {
                Id = x.Id,
                LegalName = x.LegalName,
                BrandName = x.BrandName,
                TaxOffice = x.TaxOffice,
                TaxNumber = x.TaxNumber,
                TradeRegistryNumber = x.TradeRegistryNumber,
                MersisNumber = x.MersisNumber,
                Website = x.Website,
                LogoUrl = x.LogoUrl,
                MainContactName = x.MainContactName,
                Email = x.Email,
                Phone = x.Phone,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted,
                Notes = x.Notes,
                InvoiceType = x.InvoiceType,
                IsEInvoiceUser = x.IsEInvoiceUser,
                ErpCode = x.ErpCode ?? string.Empty,
                CountryId = x.CountryId,
                BillingAddressId = x.BillingAddressId,
                InsertDate = x.InsertDate,
                UpdateDate = x.UpdateDate,
                InsertUserId = x.InsertUserId,
                UpdateUserId = x.UpdateUserId,
                DeleteUserId = x.DeleteUserId,
                DeleteDate = x.DeleteDate
            }).ToList();
        }

    }
}
