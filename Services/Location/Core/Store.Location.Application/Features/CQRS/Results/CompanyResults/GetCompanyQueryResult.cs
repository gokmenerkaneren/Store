using Store.Location.Domain.Entities;
using Store.Shared.Enums;

namespace Store.Location.Application.Features.CQRS.Results.CompanyResults
{
    public class GetCompanyQueryResult
    {
        public Guid Id { get; set; }
        public string LegalName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string TaxOffice { get; set; } = string.Empty;
        public string TaxNumber { get; set; } = string.Empty;
        public string TradeRegistryNumber { get; set; } = string.Empty;
        public string MersisNumber { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string MainContactName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string? Notes { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public bool IsEInvoiceUser { get; set; }
        public string ErpCode { get; set; } = string.Empty;
        public Guid? CountryId { get; set; }
        public Guid? BillingAddressId { get; set; }
        public DateTimeOffset InsertDate { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }
        public Guid InsertUserId { get; set; }
        public Guid? UpdateUserId { get; set; }
        public Guid? DeleteUserId { get; set; }
        public DateTimeOffset? DeleteDate { get; set; }
    }
}