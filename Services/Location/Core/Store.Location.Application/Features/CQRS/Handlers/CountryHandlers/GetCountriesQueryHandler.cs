using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using Store.Location.Application.Features.CQRS.Results.CountryResults;

namespace Store.Location.Application.Features.CQRS.Handlers.CountryHandlers
{
    public class GetCountriesQueryHandler
    {
        private readonly IRepository<Country> _repository;

        public GetCountriesQueryHandler(IRepository<Country> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetCountryQueryResult>> Handle(bool showDeleted)
        {
            var countries = await _repository.GetAllAsync(includeDeleted: showDeleted);
            var filtered = showDeleted ? countries : countries.Where(x => !x.IsDeleted);
            return filtered.Select(x => new GetCountryQueryResult
            {
                Id = x.Id,
                CountryName = x.CountryName,
                CountryCode = x.CountryCode,
                UPhoneCode = x.UPhoneCode,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted,
                Notes = x.Notes,
                InsertUserId = x.InsertUserId,
                InsertDate = x.InsertDate,
                UpdateUserId = x.UpdateUserId,
                UpdateDate = x.UpdateDate,
                DeleteUserId = x.DeleteUserId,
                DeleteDate = x.DeleteDate,
            }).ToList();
        }
    }
}

