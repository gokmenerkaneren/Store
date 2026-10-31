using Store.Location.Application.Features.CQRS.Queries.NeighborhoodQueries;
using Store.Location.Application.Features.CQRS.Results.NeighborhoodResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.NeighborhoodHandlers
{
    public class GetNeighborhoodsByDistrictIdQueryHandler
    {
        private readonly IRepository<Neighborhood> _repository;

        public GetNeighborhoodsByDistrictIdQueryHandler(IRepository<Neighborhood> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetNeighborhoodQueryResult>> Handle(GetNeighborhoodsByDistrictIdQuery query)
        {
            var neighborhoods = await _repository.GetAllByFilterAsync(x => x.DistrictId == query.DistrictId, includeDeleted: query.ShowDeleted);
            if (!query.ShowDeleted)
            {
                neighborhoods = neighborhoods.Where(n => !n.IsDeleted).ToList();
            }
            return neighborhoods.Select(x => new GetNeighborhoodQueryResult
            {
                Id = x.Id,
                NeighborhoodName = x.NeighborhoodName,
                PostalCode = x.PostalCode,
                DistrictId = x.DistrictId,
                IsActive = x.IsActive,
                Notes = x.Notes,
                InsertUserId = x.InsertUserId,
                InsertDate = x.InsertDate,
                UpdateUserId = x.UpdateUserId,
                UpdateDate = x.UpdateDate,
                IsDeleted = x.IsDeleted,
                DeleteUserId = x.DeleteUserId,
                DeleteDate = x.DeleteDate,
            }).OrderBy(x => x.NeighborhoodName).ToList();
        }
    }
}
