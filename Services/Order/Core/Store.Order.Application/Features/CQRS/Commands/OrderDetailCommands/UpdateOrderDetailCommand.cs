using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Order.Application.Features.CQRS.Commands.OrderDetailCommands
{
    public class UpdateOrderDetailCommand
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public decimal ProductPrice { get; set; } = 0;
        public int ProductAmount { get; set; } = 0;
        public decimal TotalPrice { get; set; } = 0;
        public Guid OrderingId { get; set; }
            public Guid PriceTypeId { get; set; } 
        public decimal OriginalPrice { get; set; } = 0;
        public decimal DiscountRateApplied { get; set; } = 0;
        public Guid CurrencyId { get; set; }
        public DateTimeOffset PricingSnapshotDate { get; set; }
    }
}



