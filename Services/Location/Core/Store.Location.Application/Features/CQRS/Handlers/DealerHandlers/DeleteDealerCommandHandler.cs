using Store.Location.Application.Features.CQRS.Commands.DealerCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.DealerHandlers
{
    public class DeleteDealerCommandHandler
    {
        private readonly IRepository<Dealer> _repository;

        public DeleteDealerCommandHandler(IRepository<Dealer> repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteDealerCommand command)
        {
            var dealer = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (dealer == null) return;

            await _repository.SoftDeleteAsync(dealer);
        }
    }
}
