using Store.Location.Application.Features.CQRS.Commands.CityCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.CityHandlers
{
    public class UpdateCityCommandHandler
    {
        private readonly IRepository<City> _repository;

        public UpdateCityCommandHandler(IRepository<City> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateCityCommand command)
        {
            var name = (command.CityName ?? "").Trim();
            var plate = (command.PlateCode ?? "").Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Şehir Adı boş bırakılamaz.");
            }

            var allCities = await _repository.GetAllAsync(includeDeleted: true);

            var existing = allCities.FirstOrDefault(c =>
                c.Id != command.Id && c.CountryId == command.CountryId && (
                    (!string.IsNullOrWhiteSpace(name) && string.Equals((c.CityName ?? "").Trim(), name, StringComparison.CurrentCultureIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(plate) && !string.IsNullOrWhiteSpace(c.PlateCode) && string.Equals((c.PlateCode ?? "").Trim(), plate, StringComparison.CurrentCultureIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(name) && string.Equals((c.CityName ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(plate) && !string.IsNullOrWhiteSpace(c.PlateCode) && string.Equals((c.PlateCode ?? "").Trim(), plate, StringComparison.OrdinalIgnoreCase))
                )
            );

            if (existing != null)
            {
                bool nameMatched = string.Equals((existing.CityName ?? "").Trim(), name, StringComparison.CurrentCultureIgnoreCase) ||
                                   string.Equals((existing.CityName ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase);

                string fieldLabel = nameMatched ? "Şehir Adı" : "Plaka Kodu";
                string matchedValue = nameMatched ? existing.CityName : existing.PlateCode;

                string msg = Store.Location.Application.Common.DuplicateCheckerHelper.BuildDuplicateMessage(fieldLabel, matchedValue, existing.IsDeleted, existing.IsActive);
                throw new InvalidOperationException(msg);
            }

            var city = await _repository.GetByIdAsync(command.Id);
            city.CityName = name;
            city.PlateCode = plate;
            city.CountryId = command.CountryId;
            city.Notes = command.Notes;
            city.IsActive = command.IsActive;

            await _repository.UpdateAsync(city);
        }
    }
}
