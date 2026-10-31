using Store.Location.Application.Features.CQRS.Commands.BranchAddressCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.BranchAddressHandlers
{
    public class UpdateBranchAddressCommandHandler
    {
        private readonly IRepository<BranchAddress> _repository;

        public UpdateBranchAddressCommandHandler(IRepository<BranchAddress> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateBranchAddressCommand command)
        {
            var branchAddress = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (branchAddress == null) return;

            branchAddress.Title = command.Title;
            branchAddress.ContactFullName = command.ContactFullName;
            branchAddress.PhoneNumber = command.PhoneNumber;
            branchAddress.AddressDetail = command.AddressDetail;
            branchAddress.CountryId = command.CountryId;
            branchAddress.CityId = command.CityId;
            branchAddress.DistrictId = command.DistrictId;
            branchAddress.NeighborhoodId = command.NeighborhoodId;
            branchAddress.BranchId = command.BranchId;
            branchAddress.LoadingDockInstructions = command.LoadingDockInstructions;
            branchAddress.IsActive = command.IsActive;
            branchAddress.Notes = command.Notes;
            branchAddress.UpdateUserId = Guid.Parse("00000002-0002-0002-0000-000000000002");
            branchAddress.UpdateDate = BaseEntity.GetLocalTime();

            await _repository.UpdateAsync(branchAddress);
        }
    }
}

