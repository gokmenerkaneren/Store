using Store.B2B.Application.Features.CQRS.Commands.DealerFinancialCommands;
using Store.B2B.Application.Interfaces;
using Store.B2B.Domain.Entities;

namespace Store.B2B.Application.Features.CQRS.Handlers.DealerFinancialHandlers
{
    public class RemoveDealerFinancialCommandHandler
    {
        private readonly IB2BRepository<DealerFinancial> _repository;

        public RemoveDealerFinancialCommandHandler(IB2BRepository<DealerFinancial> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveDealerFinancialCommand command)
        {
            await _repository.DeleteAsync(command.Id);
        }
    }
}
