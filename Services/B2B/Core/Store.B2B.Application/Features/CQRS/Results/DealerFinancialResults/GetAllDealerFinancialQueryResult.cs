namespace Store.B2B.Application.Features.CQRS.Results.DealerFinancialResults
{
    public class GetAllDealerFinancialQueryResult
    {
        public Guid Id { get; set; }
        public Guid DealerId { get; set; }
        public string TaxNumber { get; set; } = string.Empty;
        public string TaxOffice { get; set; } = string.Empty;
        public decimal DiscountRate { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal CurrentBalance { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
