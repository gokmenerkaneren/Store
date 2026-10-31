using Store.Location.Application.Features.CQRS.Commands.BranchCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.BranchHandlers
{
    public class RestoreBranchCommandHandler
    {
        private readonly IRepository<Branch> _repository;

        public RestoreBranchCommandHandler(IRepository<Branch> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RestoreBranchCommand command)
        {
            var branch = await _repository.GetByIdAsync(command.Id);
            if (branch == null) return;

            await _repository.RestoreAsync(branch);
        }
    }
}
