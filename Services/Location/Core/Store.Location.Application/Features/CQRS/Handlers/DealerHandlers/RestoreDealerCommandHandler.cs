using Store.Location.Application.Features.CQRS.Commands.DealerCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.DealerHandlers
{
    public class RestoreDealerCommandHandler
    {
        private readonly IRepository<Dealer> _repository;

        public RestoreDealerCommandHandler(IRepository<Dealer> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RestoreDealerCommand command)
        {
            var dealer = await _repository.GetByIdAsync(command.Id);
            if (dealer == null) return;

            await _repository.RestoreAsync(dealer);
        }
    }
}
