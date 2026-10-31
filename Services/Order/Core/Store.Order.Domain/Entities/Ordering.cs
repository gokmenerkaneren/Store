using System;
using System.Collections.Generic;
using Store.Shared.Entities;

namespace Store.Order.Domain.Entities
{
    public class Ordering : BaseEntity
    {
        public Guid UserId { get; set; }
        public decimal TotalPrice { get; set; } = 0;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
