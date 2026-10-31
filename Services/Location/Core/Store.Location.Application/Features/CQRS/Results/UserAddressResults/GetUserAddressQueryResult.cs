using System;
using Store.Shared.Enums;

namespace Store.Location.Application.Features.CQRS.Results.UserAddressResults
{
    public class GetUserAddressQueryResult
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ContactFullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AddressDetail { get; set; } = string.Empty;
        public Guid CountryId { get; set; }
        public Guid CityId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid? NeighborhoodId { get; set; }
        public Guid CustomerId { get; set; }
        public InvoiceType InvoiceType { get; set; }
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
