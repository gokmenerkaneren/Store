using Store.Location.Application.Features.CQRS.Queries.CompanyQueries;
using Store.Location.Application.Features.CQRS.Results.CompanyResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.CompanyHandlers
{
    public class GetCompaniesQueryHandler
    {
        private readonly IRepository<Company> _repository;

        public GetCompaniesQueryHandler(IRepository<Company> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetCompanyQueryResult>> Handle(bool showDeleted = false)
        {
            var companies = await _repository.GetAllAsync(includeDeleted: showDeleted);
            if (!showDeleted) companies = companies.Where(x => !x.IsDeleted).ToList();
            return companies.Select(x => new GetCompanyQueryResult
            {
                Id = x.Id,
                LegalName = x.LegalName,
                BrandName = x.BrandName,
                TaxOffice = x.TaxOffice,
                TaxNumber = x.TaxNumber,
                Website = x.Website,
                LogoUrl = x.LogoUrl,
                IsActive = x.IsActive,
                Notes = x.Notes,
                InsertUserId = x.InsertUserId,
                InsertDate = x.InsertDate,
                UpdateUserId = x.UpdateUserId,
                UpdateDate = x.UpdateDate,
                IsDeleted = x.IsDeleted,
                DeleteUserId = x.DeleteUserId,
                DeleteDate = x.DeleteDate,
            }).ToList();
        }
    }
}


