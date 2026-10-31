using Store.Location.Application.Features.CQRS.Commands.CityCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.CityHandlers
{
    public class CreateCityCommandHandler
    {
        private readonly IRepository<City> _repository;

        public CreateCityCommandHandler(IRepository<City> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateCityCommand command)
        {
            var name = (command.CityName ?? "").Trim();
            var plate = (command.PlateCode ?? "").Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Şehir Adı boş bırakılamaz.");
            }

            var allCities = await _repository.GetAllAsync(includeDeleted: true);

            var existing = allCities.FirstOrDefault(c =>
                c.CountryId == command.CountryId && (
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

            var city = new City
            {
                CityName = name,
                PlateCode = plate,
                CountryId = command.CountryId,
                Notes = command.Notes,
                IsActive = command.IsActive,
                IsDeleted = false,
                InsertUserId = Guid.Parse("00000001-0001-0001-0001-000000000001"),
                InsertDate = BaseEntity.GetLocalTime()
            };

            await _repository.CreateAsync(city);
        }
    }
}
