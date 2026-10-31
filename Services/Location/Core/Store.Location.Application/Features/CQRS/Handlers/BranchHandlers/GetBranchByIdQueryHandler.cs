using Store.Location.Application.Features.CQRS.Queries.BranchQueries;
using Store.Location.Application.Features.CQRS.Results.BranchResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.BranchHandlers
{
    public class GetBranchByIdQueryHandler
    {
        private readonly IRepository<Branch> _repository;

        public GetBranchByIdQueryHandler(IRepository<Branch> repository)
        {
            _repository = repository;
        }

        public async Task<GetBranchByIdQueryResult?> Handle(GetBranchByIdQuery query)
        {
            var values = await _repository.GetByFilterAsync(x => x.Id == query.Id);
            if (values == null) return null;

            return new GetBranchByIdQueryResult
            {
                Id = values.Id,
                Name = values.Name,
                BranchType = values.BranchType,
                CompanyId = values.CompanyId,
                ContactInfoId = values.ContactInfoId,
                GeoLocationCoordinates = values.GeoLocationCoordinates,
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

