using Store.Location.Application.Features.CQRS.Commands.BranchAddressCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.BranchAddressHandlers
{
    public class SoftDeleteBranchAddressCommandHandler
    {
        private readonly IRepository<BranchAddress> _repository;

        public SoftDeleteBranchAddressCommandHandler(IRepository<BranchAddress> repository)
        {
            _repository = repository;
        }

        public async Task Handle(SoftDeleteBranchAddressCommand command)
        {
            var branchAddress = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (branchAddress == null) return;

            await _repository.SoftDeleteAsync(branchAddress);
        }
    }
}
