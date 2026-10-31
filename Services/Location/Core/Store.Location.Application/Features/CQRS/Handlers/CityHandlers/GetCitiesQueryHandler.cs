using Store.Location.Application.Features.CQRS.Queries.CityQueries;
using Store.Location.Application.Features.CQRS.Results.CityResults;
using Store.Location.Application.Features.CQRS.Results.CountryResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.CityHandlers
{
    public class GetCitiesQueryHandler
    {
        private readonly IRepository<City> _repository;

        public GetCitiesQueryHandler(IRepository<City> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetCityQueryResult>> Handle(bool showDeleted)
        {
            var cities = await _repository.GetAllAsync(includeDeleted: showDeleted);
            var filtered = showDeleted ? cities : cities.Where(x => !x.IsDeleted);
            return filtered.Select(x => new GetCityQueryResult
            {
                Id = x.Id,
                CityName = x.CityName,
                PlateCode = x.PlateCode,
                CountryId = x.CountryId,
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

