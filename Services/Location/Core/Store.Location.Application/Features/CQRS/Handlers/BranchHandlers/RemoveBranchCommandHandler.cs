using Store.Location.Application.Features.CQRS.Commands.BranchCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.BranchHandlers
{
    public class RemoveBranchCommandHandler
    {
        private readonly IRepository<Branch> _repository;

        public RemoveBranchCommandHandler(IRepository<Branch> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveBranchCommand command)
        {
            var branch = await _repository.GetByIdAsync(command.Id);
            if (branch != null)
            {
                await _repository.SoftDeleteAsync(branch);
            }
        }
    }
}
