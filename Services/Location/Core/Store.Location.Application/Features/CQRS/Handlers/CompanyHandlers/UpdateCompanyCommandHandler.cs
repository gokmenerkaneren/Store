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
    public class UpdateCompanyCommandHandler
    {
        private readonly IRepository<Company> _repository;

        public UpdateCompanyCommandHandler(IRepository<Company> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateCompanyCommand command)
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
                c.Id != command.Id && (
                    (!string.IsNullOrWhiteSpace(legal) && string.Equals((c.LegalName ?? "").Trim(), legal, StringComparison.CurrentCultureIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(legal) && string.Equals((c.LegalName ?? "").Trim(), legal, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(brand) && !string.IsNullOrWhiteSpace(c.BrandName) && (string.Equals((c.BrandName ?? "").Trim(), brand, StringComparison.CurrentCultureIgnoreCase) || string.Equals((c.BrandName ?? "").Trim(), brand, StringComparison.OrdinalIgnoreCase))) ||
                    (!string.IsNullOrWhiteSpace(tax) && !string.IsNullOrWhiteSpace(c.TaxNumber) && (string.Equals((c.TaxNumber ?? "").Trim(), tax, StringComparison.CurrentCultureIgnoreCase) || string.Equals((c.TaxNumber ?? "").Trim(), tax, StringComparison.OrdinalIgnoreCase)))
                )
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

            var company = await _repository.GetByIdAsync(command.Id);
            company.LegalName = legal;
            company.BrandName = brand;
            company.TaxOffice = command.TaxOffice;
            company.TaxNumber = tax;
            company.TradeRegistryNumber = command.TradeRegistryNumber;
            company.MersisNumber = command.MersisNumber;
            company.Website = command.Website;
            company.LogoUrl = command.LogoUrl;
            company.MainContactName = command.MainContactName;
            company.Email = command.Email;
            company.Phone = command.Phone;
            company.Notes = command.Notes;
            company.InvoiceType = command.InvoiceType;
            company.IsEInvoiceUser = command.IsEInvoiceUser;
            company.ErpCode = command.ErpCode ?? string.Empty;
            company.CountryId = command.CountryId;
            company.BillingAddressId = command.BillingAddressId;
            company.IsActive = command.IsActive;

            await _repository.UpdateAsync(company);
        }
    }
}
