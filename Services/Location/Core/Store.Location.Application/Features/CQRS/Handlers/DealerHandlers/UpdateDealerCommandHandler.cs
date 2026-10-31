using Store.Location.Application.Features.CQRS.Commands.DealerCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.DealerHandlers
{
    public class UpdateDealerCommandHandler
    {
        private readonly IRepository<Dealer> _repository;

        public UpdateDealerCommandHandler(IRepository<Dealer> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateDealerCommand command)
        {
            var existingList = await _repository.GetAllByFilterAsync(
                d => d.Id != command.Id &&
                     (d.CompanyName.ToLower() == command.CompanyName.ToLower()),
                includeDeleted: true
            );

            var existing = existingList.FirstOrDefault();
            if (existing != null)
            {
                bool companyMatched = existing.CompanyName.ToLower() == command.CompanyName.ToLower();
                string fieldLabel = "Bayi Firma Adı";
                string matchedValue = existing.CompanyName;

                string msg = Store.Location.Application.Common.DuplicateCheckerHelper.BuildDuplicateMessage(fieldLabel, matchedValue, existing.IsDeleted, existing.IsActive);
                throw new InvalidOperationException(msg);
            }

            var dealer = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (dealer == null) return;

            dealer.CompanyName = command.CompanyName;
            dealer.ContactInfoId = command.ContactInfoId;
            dealer.IsActive = command.IsActive;
            dealer.Notes = command.Notes;
            dealer.TaxNumber = command.TaxNumber;
            dealer.TaxOffice = command.TaxOffice;
            dealer.DiscountRate = command.DiscountRate;
            dealer.CreditLimit = command.CreditLimit;
            dealer.CurrentBalance = command.CurrentBalance;
            dealer.UpdateUserId = Guid.Parse("00000002-0002-0002-0000-000000000002");
            dealer.UpdateDate = BaseEntity.GetLocalTime();

            await _repository.UpdateAsync(dealer);
        }
    }
}

