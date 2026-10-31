using System;
using Store.Shared.Enums;

namespace Store.DtoLayer.LocationDtos.UserAddressDtos
{
    public class CreateUserAddressDto
    {
        public string Title { get; set; } = string.Empty;
        public string ContactFullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AddressDetail { get; set; } = string.Empty;
        public Guid CountryId { get; set; }
        public Guid CityId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid? NeighborhoodId { get; set; }
        public Guid CustomerId { get; set; } // Matches UserId
        public InvoiceType InvoiceType { get; set; }
        public string? IdentityNumber { get; set; }
        public string? TaxOffice { get; set; }
        public string? TaxNumber { get; set; }
        public Guid? CompanyId { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public Guid InsertUserId { get; set; }
    }
}
