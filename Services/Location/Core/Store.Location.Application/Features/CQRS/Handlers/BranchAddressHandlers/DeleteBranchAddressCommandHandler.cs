using Store.Location.Application.Features.CQRS.Commands.BranchAddressCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.BranchAddressHandlers
{
    public class DeleteBranchAddressCommandHandler
    {
        private readonly IRepository<BranchAddress> _repository;

        public DeleteBranchAddressCommandHandler(IRepository<BranchAddress> repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteBranchAddressCommand command)
        {
            var branchAddress = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (branchAddress == null) return;

            await _repository.SoftDeleteAsync(branchAddress);
        }
    }
}
