using Store.Location.Application.Features.CQRS.Queries.BranchAddress;
using Store.Location.Application.Features.CQRS.Results.BranchAddress;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.BranchAddressHandlers
{
    public class GetBranchAddressByIdQueryHandler
    {
        private readonly IRepository<BranchAddress> _repository;

        public GetBranchAddressByIdQueryHandler(IRepository<BranchAddress> repository)
        {
            _repository = repository;
        }

        public async Task<GetBranchAddressByIdQueryResult?> Handle(GetBranchAddressByIdQuery query)
        {
            var values = await _repository.GetByFilterAsync(a => a.Id == query.Id);
            if (values == null) return null;

            return new GetBranchAddressByIdQueryResult
            {
                Id = values.Id,
                Title = values.Title,
                ContactFullName = values.ContactFullName,
                PhoneNumber = values.PhoneNumber,
                AddressDetail = values.AddressDetail,
                CountryId = values.CountryId,
                CityId = values.CityId,
                DistrictId = values.DistrictId,
                NeighborhoodId = values.NeighborhoodId,
                BranchId = values.BranchId,
                LoadingDockInstructions = values.LoadingDockInstructions,
                IsActive = values.IsActive,
                IsDeleted = values.IsDeleted,
                Notes = values.Notes,
                InsertDate = values.InsertDate,
                UpdateDate = values.UpdateDate,
                InsertUserId = values.InsertUserId,
                DeleteDate = values.DeleteDate,
            };
        }
    }
}

