using Store.Location.Application.Features.CQRS.Queries.DistrictQueries;
using Store.Location.Application.Features.CQRS.Results.DistrictResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.DistrictHandlers
{
    public class GetDistrictsQueryHandler
    {
        private readonly IRepository<District> _repository;

        public GetDistrictsQueryHandler(IRepository<District> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetDistrictQueryResult>> Handle(bool showDeleted)
        {
            var districts = await _repository.GetAllAsync(includeDeleted: showDeleted);
            var filtered = showDeleted ? districts : districts.Where(x => !x.IsDeleted);
            return filtered.Select(x => new GetDistrictQueryResult
            {
                Id = x.Id,
                DistrictName = x.DistrictName,
                CityId = x.CityId,
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

