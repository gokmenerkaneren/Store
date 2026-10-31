using Store.Location.Domain.Entities;
using Store.Shared.Enums;

namespace Store.Location.Application.Features.CQRS.Results.DealerAddressResults
{
    public class GetDealerAddressByIdQueryResult
    {
        public Guid Id { get; set; }
        public Guid DealerId { get; set; }
        public AddressType AddressType { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ContactFullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AddressDetail { get; set; } = string.Empty;

        // Lokasyon FK'ları
        public Guid CountryId { get; set; }
        public Guid CityId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid? NeighborhoodId { get; set; }

        // Lokasyon isimleri (UI'da görüntülemek için)
        public string? CountryName { get; set; }
        public string? CityName { get; set; }
        public string? DistrictName { get; set; }
        public string? NeighborhoodName { get; set; }

        // Birleştirilmiş adres (UI'da tek satırda göstermek için)
        public string FullFormattedAddress =>
            $"{AddressDetail}, {NeighborhoodName ?? ""} {DistrictName}/{CityName} - {CountryName}".Trim();

        // Audit
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid InsertUserId { get; set; }
        public DateTimeOffset InsertDate { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? DeleteUserId { get; set; }
        public DateTimeOffset? DeleteDate { get; set; }
    }

    public class GetDealerAddressQueryResult
    {
        public Guid Id { get; set; }
        public Guid DealerId { get; set; }
        public AddressType AddressType { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ContactFullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AddressDetail { get; set; } = string.Empty;
        public Guid CountryId { get; set; }
        public Guid CityId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid? NeighborhoodId { get; set; }
        public string? CountryName { get; set; }
        public string? CityName { get; set; }
        public string? DistrictName { get; set; }
        public string? NeighborhoodName { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public Guid InsertUserId { get; set; }
        public DateTimeOffset InsertDate { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? DeleteUserId { get; set; }
        public DateTimeOffset? DeleteDate { get; set; }
    }
}
