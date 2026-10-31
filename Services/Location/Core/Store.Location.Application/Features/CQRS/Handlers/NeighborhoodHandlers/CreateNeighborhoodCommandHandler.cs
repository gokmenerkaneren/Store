using Store.Location.Application.Features.CQRS.Commands.NeighborhoodCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.NeighborhoodHandlers
{
    public class CreateNeighborhoodCommandHandler
    {
        private readonly IRepository<Neighborhood> _repository;
        private readonly IRepository<District> _districtRepository;
        private readonly IRepository<City> _cityRepository;

        public CreateNeighborhoodCommandHandler(
            IRepository<Neighborhood> repository,
            IRepository<District> districtRepository,
            IRepository<City> cityRepository)
        {
            _repository = repository;
            _districtRepository = districtRepository;
            _cityRepository = cityRepository;
        }

        public async Task Handle(CreateNeighborhoodCommand command)
        {
            var name = (command.NeighborhoodName ?? "").Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Mahalle Adı boş bırakılamaz.");
            }

            var all = await _repository.GetAllAsync(includeDeleted: true);

            // 1) Aynı ilçede aynı mahalle adı duplicate kontrolü
            var existing = all.FirstOrDefault(n =>
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

            // 2) Aynı ülkede aynı posta kodu duplicate kontrolü
            var postalCode = (command.PostalCode ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(postalCode))
            {
                // Yeni kaydın ülkesini bul: DistrictId → District.CityId → City.CountryId
                var newDistrict = await _districtRepository.GetByIdAsync(command.DistrictId);
                var newCity = newDistrict != null ? await _cityRepository.GetByIdAsync(newDistrict.CityId) : null;
                var newCountryId = newCity?.CountryId;

                if (newCountryId.HasValue)
                {
                    // Tüm districts ve cities'i tek seferde yükle (N+1 sorgudan kaçın)
                    var allDistricts = await _districtRepository.GetAllAsync(includeDeleted: true);
                    var allCities = await _cityRepository.GetAllAsync(includeDeleted: true);

                    var existingPostal = all.FirstOrDefault(n =>
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

            var neighborhood = new Neighborhood
            {
                NeighborhoodName = name,
                DistrictId = command.DistrictId,
                PostalCode = string.IsNullOrWhiteSpace(postalCode) ? null : postalCode,
                Notes = command.Notes,
                IsActive = true,
                IsDeleted = false,
                InsertUserId = Guid.Parse("00000001-0001-0001-0001-000000000001"),
                InsertDate = BaseEntity.GetLocalTime()
            };

            await _repository.CreateAsync(neighborhood);
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
