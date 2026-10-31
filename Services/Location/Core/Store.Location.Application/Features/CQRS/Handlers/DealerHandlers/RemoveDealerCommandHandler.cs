using Store.Location.Application.Features.CQRS.Commands.DealerCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.DealerHandlers
{
    public class RemoveDealerCommandHandler
    {
        private readonly IRepository<Dealer> _repository;

        public RemoveDealerCommandHandler(IRepository<Dealer> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveDealerCommand command)
        {
            var dealer = await _repository.GetByIdAsync(command.Id);
            if (dealer != null)
            {
                await _repository.SoftDeleteAsync(dealer);
            }
        }
    }
}
