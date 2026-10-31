using System;

namespace Store.DtoLayer.B2BDtos
{
    public class ResultDealerFinancialDto
    {
        public Guid Id { get; set; }
        public Guid DealerId { get; set; }
        public string TaxNumber { get; set; } = string.Empty;
        public string TaxOffice { get; set; } = string.Empty;
        public decimal DiscountRate { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal CurrentBalance { get; set; }
    }
}
