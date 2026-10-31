using Store.Location.Application.Features.CQRS.Queries.NeighborhoodQueries;
using Store.Location.Application.Features.CQRS.Results.NeighborhoodResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.NeighborhoodHandlers
{
    public class GetNeighborhoodByIdQueryHandler
    {
        private readonly IRepository<Neighborhood> _repository;

        public GetNeighborhoodByIdQueryHandler(IRepository<Neighborhood> repository)
        {
            _repository = repository;
        }

        public async Task<GetNeighborhoodByIdQueryResult?> Handle(GetNeighborhoodByIdQuery query)
        {
            var values = await _repository.GetByFilterAsync(x => x.Id == query.Id);
            if (values == null) return null;

            return new GetNeighborhoodByIdQueryResult
            {
                Id = values.Id,
                NeighborhoodName = values.NeighborhoodName,
                PostalCode = values.PostalCode,
                DistrictId = values.DistrictId,
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

