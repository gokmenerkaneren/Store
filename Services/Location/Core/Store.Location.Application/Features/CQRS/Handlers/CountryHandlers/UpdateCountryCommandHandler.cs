using Store.Location.Application.Features.CQRS.Commands.CountryCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.CountryHandlers
{
    public class UpdateCountryCommandHandler
    {
        private readonly IRepository<Country> _repository;

        public UpdateCountryCommandHandler(IRepository<Country> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateCountryCommand command)
        {
            var name = (command.CountryName ?? "").Trim();
            var code = (command.CountryCode ?? "").Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Ülke Adı boş bırakılamaz.");
            }

            var allCountries = await _repository.GetAllAsync(includeDeleted: true);

            var existing = allCountries.FirstOrDefault(c =>
                c.Id != command.Id && (
                    (!string.IsNullOrWhiteSpace(name) && string.Equals((c.CountryName ?? "").Trim(), name, StringComparison.CurrentCultureIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(code) && !string.IsNullOrWhiteSpace(c.CountryCode) && string.Equals((c.CountryCode ?? "").Trim(), code, StringComparison.CurrentCultureIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(name) && string.Equals((c.CountryName ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(code) && !string.IsNullOrWhiteSpace(c.CountryCode) && string.Equals((c.CountryCode ?? "").Trim(), code, StringComparison.OrdinalIgnoreCase))
                )
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

            var country = await _repository.GetByIdAsync(command.Id);
            country.CountryName = name;
            country.CountryCode = code;
            country.Notes = command.Notes;
            country.IsActive = command.IsActive;

            await _repository.UpdateAsync(country);
        }
    }
}
