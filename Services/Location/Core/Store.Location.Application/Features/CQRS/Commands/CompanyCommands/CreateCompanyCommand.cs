using Store.Location.Domain.Entities;
using Store.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using Store.Shared.Enums;

namespace Store.Location.Application.Features.CQRS.Commands.CompanyCommands
{
    public class CreateCompanyCommand
    {
        [Required, StringLength(250)]
        public string LegalName { get; set; } = string.Empty;

        [Required, StringLength(250)]
        public string BrandName { get; set; } = string.Empty;

        [StringLength(100)]
        public string TaxOffice { get; set; } = string.Empty;

        [StringLength(15)]
        public string TaxNumber { get; set; } = string.Empty;

        [StringLength(100)]
        public string TradeRegistryNumber { get; set; } = string.Empty;

        [StringLength(30)]
        public string MersisNumber { get; set; } = string.Empty;

        [StringLength(200)]
        public string Website { get; set; } = string.Empty;

        [StringLength(500)]
        public string LogoUrl { get; set; } = string.Empty;

        public string MainContactName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Notes { get; set; }
        
        public InvoiceType InvoiceType { get; set; }
        public bool IsEInvoiceUser { get; set; }
        
        [StringLength(50)]
        public string ErpCode { get; set; } = string.Empty;
        
        public Guid? CountryId { get; set; }
        public Guid? BillingAddressId { get; set; }
    }
}
