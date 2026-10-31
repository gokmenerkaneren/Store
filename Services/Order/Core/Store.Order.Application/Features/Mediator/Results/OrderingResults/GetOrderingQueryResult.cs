using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Order.Application.Features.Mediator.Results.OrderingResults
{
    public class GetOrderingQueryResult
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal TotalPrice { get; set; } = 0;
        public DateTime OrderDate { get; set; } = DateTime.Now;
    }
}


