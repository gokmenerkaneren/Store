using Store.Location.Application.Features.CQRS.Commands.DealerCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.DealerHandlers
{
    public class CreateDealerCommandHandler
    {
        private readonly IRepository<Dealer> _repository;

        public CreateDealerCommandHandler(IRepository<Dealer> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateDealerCommand command)
        {
            var existingList = await _repository.GetAllByFilterAsync(
                d => d.CompanyName.ToLower() == command.CompanyName.ToLower(),
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

            var dealer = new Dealer
            {
                CompanyName = command.CompanyName,
                ContactInfoId = command.ContactInfoId,
                Notes = command.Notes,
                TaxNumber = command.TaxNumber,
                TaxOffice = command.TaxOffice,
                DiscountRate = command.DiscountRate,
                CreditLimit = command.CreditLimit,
                CurrentBalance = command.CurrentBalance,
                IsActive = true,
                IsDeleted = false,
                InsertUserId = Guid.Parse("00000001-0001-0001-0001-000000000001"),
                InsertDate = BaseEntity.GetLocalTime()
            };

            await _repository.CreateAsync(dealer);
        }
    }
}

