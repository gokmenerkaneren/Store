using System.ComponentModel.DataAnnotations;

namespace Store.Location.Application.Features.CQRS.Commands.DealerCommands
{
    public class CreateDealerCommand
    {
        [Required, StringLength(250)]
        public string CompanyName { get; set; } = string.Empty;


        public Guid? ContactInfoId { get; set; }
        public string? Notes { get; set; }
        public string TaxNumber { get; set; } = string.Empty;
        public string TaxOffice { get; set; } = string.Empty;
        public decimal DiscountRate { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal CurrentBalance { get; set; }
    }
}
