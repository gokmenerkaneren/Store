using Store.Location.Application.Features.CQRS.Commands.BranchCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.BranchHandlers
{
    public class DeleteBranchCommandHandler
    {
        private readonly IRepository<Branch> _repository;

        public DeleteBranchCommandHandler(IRepository<Branch> repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteBranchCommand command)
        {
            var branch = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (branch == null) return;

            await _repository.SoftDeleteAsync(branch);
        }
    }
}
