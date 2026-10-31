using Store.Location.Application.Features.CQRS.Commands.CompanyCommands;
using Store.Shared.Enums;
using Store.Location.Application.Interfaces;
using Store.Shared.Enums;
using Store.Location.Domain.Entities;
using Store.Shared.Enums;
using System;
using Store.Shared.Enums;
using System.Linq;
using Store.Shared.Enums;
using System.Threading.Tasks;
using Store.Shared.Enums;

namespace Store.Location.Application.Features.CQRS.Handlers.CompanyHandlers
{
    public class CreateCompanyCommandHandler
    {
        private readonly IRepository<Company> _repository;

        public CreateCompanyCommandHandler(IRepository<Company> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateCompanyCommand command)
        {
            var legal = (command.LegalName ?? "").Trim();
            var brand = (command.BrandName ?? "").Trim();
            var tax = (command.TaxNumber ?? "").Trim();

            if (string.IsNullOrWhiteSpace(legal))
            {
                throw new InvalidOperationException("Firma Unvanı boş bırakılamaz.");
            }

            var all = await _repository.GetAllAsync(includeDeleted: true);

            var existing = all.FirstOrDefault(c =>
                (!string.IsNullOrWhiteSpace(legal) && string.Equals((c.LegalName ?? "").Trim(), legal, StringComparison.CurrentCultureIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(legal) && string.Equals((c.LegalName ?? "").Trim(), legal, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(brand) && !string.IsNullOrWhiteSpace(c.BrandName) && (string.Equals((c.BrandName ?? "").Trim(), brand, StringComparison.CurrentCultureIgnoreCase) || string.Equals((c.BrandName ?? "").Trim(), brand, StringComparison.OrdinalIgnoreCase))) ||
                (!string.IsNullOrWhiteSpace(tax) && !string.IsNullOrWhiteSpace(c.TaxNumber) && (string.Equals((c.TaxNumber ?? "").Trim(), tax, StringComparison.CurrentCultureIgnoreCase) || string.Equals((c.TaxNumber ?? "").Trim(), tax, StringComparison.OrdinalIgnoreCase)))
            );

            if (existing != null)
            {
                bool legalMatched = string.Equals((existing.LegalName ?? "").Trim(), legal, StringComparison.CurrentCultureIgnoreCase) || string.Equals((existing.LegalName ?? "").Trim(), legal, StringComparison.OrdinalIgnoreCase);
                bool brandMatched = !string.IsNullOrWhiteSpace(brand) && (string.Equals((existing.BrandName ?? "").Trim(), brand, StringComparison.CurrentCultureIgnoreCase) || string.Equals((existing.BrandName ?? "").Trim(), brand, StringComparison.OrdinalIgnoreCase));
                
                string fieldLabel = legalMatched ? "Firma Unvanı" : (brandMatched ? "Marka Adı" : "Vergi Numarası");
                string matchedValue = legalMatched ? existing.LegalName : (brandMatched ? (existing.BrandName ?? "") : (existing.TaxNumber ?? ""));

                string msg = Store.Location.Application.Common.DuplicateCheckerHelper.BuildDuplicateMessage(fieldLabel, matchedValue, existing.IsDeleted, existing.IsActive);
                throw new InvalidOperationException(msg);
            }

            var company = new Company
            {
                LegalName = legal,
                BrandName = brand,
                TaxOffice = command.TaxOffice,
                TaxNumber = tax,
                TradeRegistryNumber = command.TradeRegistryNumber,
                MersisNumber = command.MersisNumber,
                Website = command.Website,
                LogoUrl = command.LogoUrl,
                MainContactName = command.MainContactName,
                Email = command.Email,
                Phone = command.Phone,
                Notes = command.Notes,
                InvoiceType = command.InvoiceType,
                IsEInvoiceUser = command.IsEInvoiceUser,
                ErpCode = command.ErpCode ?? string.Empty,
                CountryId = command.CountryId,
                BillingAddressId = command.BillingAddressId,
                IsActive = true,
                IsDeleted = false,
                InsertUserId = Guid.Parse("00000001-0001-0001-0001-000000000001"),
                InsertDate = BaseEntity.GetLocalTime()
            };

            await _repository.CreateAsync(company);
        }
    }
}
