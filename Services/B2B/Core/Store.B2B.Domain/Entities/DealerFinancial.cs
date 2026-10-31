using System;
using System.ComponentModel.DataAnnotations;
using Store.Shared.Entities;

namespace Store.B2B.Domain.Entities
{
    public class DealerFinancial : BaseEntity
    {
        public Guid DealerId { get; set; } // Reference to Dealer in Location/Identity

        [Required, StringLength(50)]
        public string TaxNumber { get; set; } = string.Empty;

        [StringLength(100)]
        public string TaxOffice { get; set; } = string.Empty;

        public decimal DiscountRate { get; set; }
        
        public decimal CreditLimit { get; set; }
        
        public decimal CurrentBalance { get; set; }
    }
}
