using Store.Location.Application.Features.CQRS.Queries.CityQueries;
using Store.Location.Application.Features.CQRS.Results.CityResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.CityHandlers
{
    public class GetCityByIdQueryHandler
    {
        private readonly IRepository<City> _repository;

        public GetCityByIdQueryHandler(IRepository<City> repository)
        {
            _repository = repository;
        }

        public async Task<GetCityByIdQueryResult?> Handle(GetCityByIdQuery query)
        {
            var values = await _repository.GetByFilterAsync(x => x.Id == query.Id);
            if (values == null) return null;

            return new GetCityByIdQueryResult
            {
                Id = values.Id,
                CityName = values.CityName,
                PlateCode = values.PlateCode,
                CountryId = values.CountryId,
                IsActive = values.IsActive,
                IsDeleted = values.IsDeleted,
                Notes = values.Notes,
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

