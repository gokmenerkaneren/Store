using Store.B2B.Application.Features.CQRS.Commands.DealerFinancialCommands;
using Store.B2B.Application.Interfaces;
using Store.B2B.Domain.Entities;
using Store.Shared.Entities;

namespace Store.B2B.Application.Features.CQRS.Handlers.DealerFinancialHandlers
{
    public class UpdateDealerFinancialCommandHandler
    {
        private readonly IB2BRepository<DealerFinancial> _repository;

        public UpdateDealerFinancialCommandHandler(IB2BRepository<DealerFinancial> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateDealerFinancialCommand command)
        {
            var entity = await _repository.GetByIdAsync(command.Id);
            if (entity is null) return;

            entity.TaxNumber = command.TaxNumber;
            entity.TaxOffice = command.TaxOffice;
            entity.DiscountRate = command.DiscountRate;
            entity.CreditLimit = command.CreditLimit;
            entity.CurrentBalance = command.CurrentBalance;
            entity.UpdateDate = BaseEntity.GetLocalTime();

            await _repository.UpdateAsync(entity);
        }
    }
}
