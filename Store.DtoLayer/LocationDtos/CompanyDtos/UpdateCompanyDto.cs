using System;

namespace Store.DtoLayer.LocationDtos.CompanyDtos
{
    public class UpdateCompanyDto
    {
        public Guid Id { get; set; }
        public string LegalName { get; set; }
        public string BrandName { get; set; }
        public string TaxOffice { get; set; }
        public string TaxNumber { get; set; }
        public string TradeRegistryNumber { get; set; }
        public string MersisNumber { get; set; }
        public string Website { get; set; }
        public string LogoUrl { get; set; }
        public string MainContactName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public int InvoiceType { get; set; }
        public bool IsEInvoiceUser { get; set; }
        public string? ErpCode { get; set; }
        public Guid? CountryId { get; set; }
        public Guid? BillingAddressId { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
    }
}
