using System;

namespace Store.DtoLayer.LocationDtos.UserAddressDtos
{
    public class CreateUserAddressDto
    {
        public string Title { get; set; }
        public string ContactFullName { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressDetail { get; set; }
        public Guid CountryId { get; set; } = Guid.Parse("dbcf7622-7a82-4439-84d6-ff571b8ef053");
        public Guid CityId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid? NeighborhoodId { get; set; }
        public Guid CustomerId { get; set; }
        public int InvoiceType { get; set; }
        public string? IdentityNumber { get; set; }
        public string? TaxOffice { get; set; }
        public string? TaxNumber { get; set; }
        public Guid? CompanyId { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; } = false;
        public string? Notes { get; set; }
        public Guid InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; } = DateTime.Now;
    }
}
