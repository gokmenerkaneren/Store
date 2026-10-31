using Store.Location.Application.Features.CQRS.Commands.CountryCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.CountryHandlers
{
    public class CreateCountryCommandHandler
    {
        private readonly IRepository<Country> _repository;

        public CreateCountryCommandHandler(IRepository<Country> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateCountryCommand command)
        {
            var name = (command.CountryName ?? "").Trim();
            var code = (command.CountryCode ?? "").Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Ülke Adı boş bırakılamaz.");
            }

            var allCountries = await _repository.GetAllAsync(includeDeleted: true);

            var existing = allCountries.FirstOrDefault(c =>
                (!string.IsNullOrWhiteSpace(name) && string.Equals((c.CountryName ?? "").Trim(), name, StringComparison.CurrentCultureIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(code) && !string.IsNullOrWhiteSpace(c.CountryCode) && string.Equals((c.CountryCode ?? "").Trim(), code, StringComparison.CurrentCultureIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(name) && string.Equals((c.CountryName ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(code) && !string.IsNullOrWhiteSpace(c.CountryCode) && string.Equals((c.CountryCode ?? "").Trim(), code, StringComparison.OrdinalIgnoreCase))
            );

            if (existing != null)
            {
                bool nameMatched = string.Equals((existing.CountryName ?? "").Trim(), name, StringComparison.CurrentCultureIgnoreCase) ||
                                   string.Equals((existing.CountryName ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase);

                string fieldLabel = nameMatched ? "Ülke Adı" : "Plaka Kodu";
                string matchedValue = nameMatched ? existing.CountryName : existing.CountryCode;

                string msg = Store.Location.Application.Common.DuplicateCheckerHelper.BuildDuplicateMessage(fieldLabel, matchedValue, existing.IsDeleted, existing.IsActive);
                throw new InvalidOperationException(msg);
            }

            var country = new Country
            {
                CountryName = name,
                CountryCode = code,
                UPhoneCode = command.UPhoneCode,
                Notes = command.Notes,
                IsActive = true,
                IsDeleted = false,
                InsertUserId = Guid.Parse("00000001-0001-0001-0001-000000000001"),
                InsertDate = BaseEntity.GetLocalTime()
            };

            await _repository.CreateAsync(country);
        }
    }
}
