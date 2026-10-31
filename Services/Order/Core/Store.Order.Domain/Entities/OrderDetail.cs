using System;
using System.Collections.Generic;
using Store.Shared.Entities;

namespace Store.Order.Domain.Entities
{
    public class OrderDetail : BaseEntity
    { 
        public Guid ProductId  { get; set; }
        public decimal ProductPrice { get; set; } = 0;
        public int ProductAmount { get; set; } = 0;
        public decimal TotalPrice { get; set; } = 0; 
        
        // Pricing Snapshot Fields
        public Guid PriceTypeId { get; set; } 
        public decimal OriginalPrice { get; set; } = 0; // İskonto öncesi baz fiyat
        public decimal DiscountRateApplied { get; set; } = 0; // Uygulanan B2B/Kampanya iskonto oranı
        public Guid CurrencyId { get; set; } // Hangi para birimiyle alındı
        public DateTimeOffset PricingSnapshotDate { get; set; } // Fiyatın hesaplandığı ve dondurulduğu an
        
        // Rental Fields
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }
        public int? RentalDays { get; set; }

        public Guid OrderingId { get; set; }
        public Ordering? Ordering { get; set; }
    }
}
