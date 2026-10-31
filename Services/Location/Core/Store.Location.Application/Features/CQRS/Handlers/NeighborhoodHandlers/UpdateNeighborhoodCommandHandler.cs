using Store.Location.Application.Features.CQRS.Commands.NeighborhoodCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.NeighborhoodHandlers
{
    public class UpdateNeighborhoodCommandHandler
    {
        private readonly IRepository<Neighborhood> _repository;
        private readonly IRepository<District> _districtRepository;
        private readonly IRepository<City> _cityRepository;

        public UpdateNeighborhoodCommandHandler(
            IRepository<Neighborhood> repository,
            IRepository<District> districtRepository,
            IRepository<City> cityRepository)
        {
            _repository = repository;
            _districtRepository = districtRepository;
            _cityRepository = cityRepository;
        }

        public async Task Handle(UpdateNeighborhoodCommand command)
        {
            var name = (command.NeighborhoodName ?? "").Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Mahalle Adı boş bırakılamaz.");
            }

            var all = await _repository.GetAllAsync(includeDeleted: true);

            // 1) Aynı ilçede aynı mahalle adı duplicate kontrolü (kendisi hariç)
            var existing = all.FirstOrDefault(n =>
                n.Id != command.Id &&
                n.DistrictId == command.DistrictId && (
                    string.Equals((n.NeighborhoodName ?? "").Trim(), name, StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals((n.NeighborhoodName ?? "").Trim(), name, StringComparison.OrdinalIgnoreCase)
                )
            );

            if (existing != null)
            {
                string msg = Store.Location.Application.Common.DuplicateCheckerHelper.BuildDuplicateMessage("Mahalle Adı", existing.NeighborhoodName, existing.IsDeleted, existing.IsActive);
                throw new InvalidOperationException(msg);
            }

            // 2) Aynı ülkede aynı posta kodu duplicate kontrolü (kendisi hariç)
            var postalCode = (command.PostalCode ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(postalCode))
            {
                // Güncellenen kaydın ülkesini bul: DistrictId → District.CityId → City.CountryId
                var newDistrict = await _districtRepository.GetByIdAsync(command.DistrictId);
                var newCity = newDistrict != null ? await _cityRepository.GetByIdAsync(newDistrict.CityId) : null;
                var newCountryId = newCity?.CountryId;

                if (newCountryId.HasValue)
                {
                    // Tüm districts ve cities'i tek seferde yükle
                    var allDistricts = await _districtRepository.GetAllAsync(includeDeleted: true);
                    var allCities = await _cityRepository.GetAllAsync(includeDeleted: true);

                    var existingPostal = all.FirstOrDefault(n =>
                        n.Id != command.Id &&
                        string.Equals((n.PostalCode ?? "").Trim(), postalCode, StringComparison.OrdinalIgnoreCase) &&
                        GetCountryIdForNeighborhood(n, allDistricts, allCities) == newCountryId.Value
                    );

                    if (existingPostal != null)
                    {
                        string msg = Store.Location.Application.Common.DuplicateCheckerHelper.BuildDuplicateMessage(
                            "Posta Kodu", existingPostal.PostalCode, existingPostal.IsDeleted, existingPostal.IsActive);
                        throw new InvalidOperationException(msg);
                    }
                }
            }

            var neighborhood = await _repository.GetByIdAsync(command.Id);
            neighborhood.NeighborhoodName = name;
            neighborhood.DistrictId = command.DistrictId;
            neighborhood.PostalCode = string.IsNullOrWhiteSpace(postalCode) ? null : postalCode;
            neighborhood.Notes = command.Notes;
            neighborhood.IsActive = command.IsActive;

            await _repository.UpdateAsync(neighborhood);
        }

        /// <summary>
        /// Bir Neighborhood kaydının bağlı olduğu CountryId'yi döndürür.
        /// Neighborhood → District → City → Country zinciri üzerinden hesaplanır.
        /// </summary>
        private static Guid? GetCountryIdForNeighborhood(
            Neighborhood neighborhood,
            System.Collections.Generic.List<District> allDistricts,
            System.Collections.Generic.List<City> allCities)
        {
            var district = allDistricts.FirstOrDefault(d => d.Id == neighborhood.DistrictId);
            if (district == null) return null;
            var city = allCities.FirstOrDefault(c => c.Id == district.CityId);
            return city?.CountryId;
        }
    }
}
