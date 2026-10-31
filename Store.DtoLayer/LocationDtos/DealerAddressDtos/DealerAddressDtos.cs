using System;

namespace Store.DtoLayer.LocationDtos.DealerAddressDtos
{
    public class CreateDealerAddressDto
    {
        public Guid DealerId { get; set; }
        public int AddressType { get; set; } = 1; // 1: Headquarters, 2: Billing, 3: Shipping
        public string Title { get; set; } = string.Empty;
        public string ContactFullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AddressDetail { get; set; } = string.Empty;
        public Guid CountryId { get; set; }
        public Guid CityId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid? NeighborhoodId { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateDealerAddressDto
    {
        public Guid Id { get; set; }
        public Guid DealerId { get; set; }
        public int AddressType { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ContactFullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AddressDetail { get; set; } = string.Empty;
        public Guid CountryId { get; set; }
        public Guid CityId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid? NeighborhoodId { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }

    public class ResultDealerAddressDto
    {
        public Guid Id { get; set; }
        public Guid DealerId { get; set; }
        public int AddressType { get; set; }
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
    }
}
