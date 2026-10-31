using Store.Location.Application.Features.CQRS.Commands.BranchAddressCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.BranchAddressHandlers
{
    public class RemoveBranchAddressCommandHandler
    {
        private readonly IRepository<BranchAddress> _repository;

        public RemoveBranchAddressCommandHandler(IRepository<BranchAddress> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveBranchAddressCommand command)
        {
            var branchAddress = await _repository.GetByIdAsync(command.Id);
            if (branchAddress != null)
            {
                await _repository.SoftDeleteAsync(branchAddress);
            }
        }
    }
}
