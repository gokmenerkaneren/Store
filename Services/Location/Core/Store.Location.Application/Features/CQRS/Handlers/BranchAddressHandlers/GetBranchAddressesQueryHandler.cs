using Store.Location.Application.Features.CQRS.Queries.BranchAddress;
using Store.Location.Application.Features.CQRS.Results.BranchAddress;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.BranchAddressHandlers
{
    public class GetBranchAddressesQueryHandler
    {
        private readonly IRepository<BranchAddress> _repository;

        public GetBranchAddressesQueryHandler(IRepository<BranchAddress> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetBranchAddressQueryResult>> Handle(bool showDeleted = false)
        {
            var list = await _repository.GetAllAsync(includeDeleted: showDeleted);
            if (!showDeleted) list = list.Where(x => !x.IsDeleted).ToList();
            return list.Select(x => new GetBranchAddressQueryResult
            {
                Id = x.Id,
                Title = x.Title,
                ContactFullName = x.ContactFullName,
                PhoneNumber = x.PhoneNumber,
                AddressDetail = x.AddressDetail,
                CountryId = x.CountryId,
                CityId = x.CityId,
                DistrictId = x.DistrictId,
                NeighborhoodId = x.NeighborhoodId,
                BranchId = x.BranchId,
                LoadingDockInstructions = x.LoadingDockInstructions,
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


