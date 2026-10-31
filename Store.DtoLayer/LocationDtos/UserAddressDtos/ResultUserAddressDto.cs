using System;

namespace Store.DtoLayer.LocationDtos.UserAddressDtos
{
    public class ResultUserAddressDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ContactFullName { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressDetail { get; set; }
        public Guid CountryId { get; set; }
        public Guid CityId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid? NeighborhoodId { get; set; }
        public Guid CustomerId { get; set; }
        public int InvoiceType { get; set; }
        public string? IdentityNumber { get; set; }
        public string? TaxOffice { get; set; }
        public string? TaxNumber { get; set; }
        public Guid? CompanyId { get; set; }
        public bool IsAnonymized { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
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
