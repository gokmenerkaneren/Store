using System;
using System.Collections.Generic;
using Store.Order.Domain.Enums;
using Store.Shared.Entities;

namespace Store.Order.Domain.Entities
{
    public class Ordering : BaseEntity
    {
        public Guid UserId { get; set; }
        public decimal TotalPrice { get; set; } = 0;
        public DateTime OrderDate { get; set; } = DateTime.Now;

        /// <summary>
        /// İ3 Sipariş Durum Makinesi
        /// </summary>
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
