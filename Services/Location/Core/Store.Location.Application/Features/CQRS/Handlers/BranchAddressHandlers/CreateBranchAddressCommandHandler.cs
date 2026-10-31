using Store.Location.Application.Features.CQRS.Commands.BranchAddressCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.BranchAddressHandlers
{
    public class CreateBranchAddressCommandHandler
    {
        private readonly IRepository<BranchAddress> _repository;

        public CreateBranchAddressCommandHandler(IRepository<BranchAddress> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateBranchAddressCommand command)
        {
            var branchAddress = new BranchAddress
            {
                Title = command.Title,
                ContactFullName = command.ContactFullName,
                PhoneNumber = command.PhoneNumber,
                AddressDetail = command.AddressDetail,
                CountryId = command.CountryId,
                CityId = command.CityId,
                DistrictId = command.DistrictId,
                NeighborhoodId = command.NeighborhoodId,
                BranchId = command.BranchId,
                LoadingDockInstructions = command.LoadingDockInstructions,
                Notes = command.Notes,
                IsActive = true,
                IsDeleted = false,
                InsertUserId = Guid.Parse("00000001-0001-0001-0001-000000000001"),
                InsertDate = BaseEntity.GetLocalTime()
            };

            await _repository.CreateAsync(branchAddress);
        }
    }
}

