using Store.Location.Application.Features.CQRS.Queries.CountryQueries;
using Store.Location.Application.Features.CQRS.Results.CountryResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.CountryHandlers
{
    public class GetCountryByIdQueryHandler
    {
        private readonly IRepository<Country> _repository;

        public GetCountryByIdQueryHandler(IRepository<Country> repository)
        {
            _repository = repository;
        }

        public async Task<GetCountryByIdQueryResult?> Handle(GetCountryByIdQuery query)
        {
            var values = await _repository.GetByFilterAsync(x => x.Id == query.Id);
            if (values == null) return null;

            return new GetCountryByIdQueryResult
            {
                Id = values.Id,
                CountryName = values.CountryName,
                CountryCode = values.CountryCode,
                UPhoneCode = values.UPhoneCode,
                IsActive = values.IsActive,
                IsDeleted = values.IsDeleted,
                Notes = values.Notes,
                InsertUserId = values.InsertUserId,
                InsertDate = values.InsertDate,
                UpdateUserId = values.UpdateUserId,
                UpdateDate = values.UpdateDate,
                DeleteUserId = values.DeleteUserId,
                DeleteDate = values.DeleteDate,
            };
        }
    }
}


