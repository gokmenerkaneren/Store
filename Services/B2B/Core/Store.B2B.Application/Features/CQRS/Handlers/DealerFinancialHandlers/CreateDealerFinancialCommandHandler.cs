using Store.B2B.Application.Features.CQRS.Commands.DealerFinancialCommands;
using Store.B2B.Application.Interfaces;
using Store.B2B.Domain.Entities;

namespace Store.B2B.Application.Features.CQRS.Handlers.DealerFinancialHandlers
{
    public class CreateDealerFinancialCommandHandler
    {
        private readonly IB2BRepository<DealerFinancial> _repository;

        public CreateDealerFinancialCommandHandler(IB2BRepository<DealerFinancial> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateDealerFinancialCommand command)
        {
            var entity = new DealerFinancial
            {
                DealerId = command.DealerId,
                TaxNumber = command.TaxNumber,
                TaxOffice = command.TaxOffice,
                DiscountRate = command.DiscountRate,
                CreditLimit = command.CreditLimit,
                CurrentBalance = command.CurrentBalance
            };
            await _repository.CreateAsync(entity);
        }
    }
}
