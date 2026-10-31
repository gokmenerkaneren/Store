using Store.Location.Application.Features.CQRS.Queries.DistrictQueries;
using Store.Location.Application.Features.CQRS.Results.DistrictResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.DistrictHandlers
{
    public class GetDistrictByIdQueryHandler
    {
        private readonly IRepository<District> _repository;

        public GetDistrictByIdQueryHandler(IRepository<District> repository)
        {
            _repository = repository;
        }

        public async Task<GetDistrictByIdQueryResult?> Handle(GetDistrictByIdQuery query)
        {
            var values = await _repository.GetByFilterAsync(x => x.Id == query.Id);
            if (values == null) return null;

            return new GetDistrictByIdQueryResult
            {
                Id = values.Id,
                DistrictName = values.DistrictName,
                CityId = values.CityId,
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

