using Store.Location.Application.Features.CQRS.Queries.NeighborhoodQueries;
using Store.Location.Application.Features.CQRS.Results.NeighborhoodResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.NeighborhoodHandlers
{
    public class GetNeighborhoodsQueryHandler
    {
        private readonly IRepository<Neighborhood> _repository;

        public GetNeighborhoodsQueryHandler(IRepository<Neighborhood> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetNeighborhoodQueryResult>> Handle(bool showDeleted = false)
        {
            var neighborhoods = await _repository.GetAllAsync(includeDeleted: showDeleted);
            if (!showDeleted)
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
            }).ToList();
        }
    }
}

