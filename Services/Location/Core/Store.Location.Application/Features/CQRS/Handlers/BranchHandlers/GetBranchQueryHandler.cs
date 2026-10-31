using Store.Location.Application.Features.CQRS.Queries.BranchQueries;
using Store.Location.Application.Features.CQRS.Results.BranchResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.BranchHandlers
{
    public class GetBranchQueryHandler
    {
        private readonly IRepository<Branch> _repository;

        public GetBranchQueryHandler(IRepository<Branch> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetBranchQueryResult>> Handle(bool showDeleted = false)
        {
            var branches = await _repository.GetAllAsync(includeDeleted: showDeleted);
            if (!showDeleted) branches = branches.Where(x => !x.IsDeleted).ToList();
            return branches.Select(x => new GetBranchQueryResult
            {
                Id = x.Id,
                Name = x.Name,
                BranchType = x.BranchType,
                CompanyId = x.CompanyId,
                ContactInfoId = x.ContactInfoId,
                GeoLocationCoordinates = x.GeoLocationCoordinates,
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


