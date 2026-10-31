namespace Store.B2B.Application.Features.CQRS.Commands.DealerFinancialCommands
{
    public class CreateDealerFinancialCommand
    {
        public Guid DealerId { get; set; }
        public string TaxNumber { get; set; } = string.Empty;
        public string TaxOffice { get; set; } = string.Empty;
        public decimal DiscountRate { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal CurrentBalance { get; set; }
    }
}
