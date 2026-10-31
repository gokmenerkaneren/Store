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

        /// <summary>
        /// Mevcut durumdan verilen duruma geçişe izin verilip verilmediğini döner.
        /// Cancelled, Completed ve Returned terminal durumlardır.
        /// </summary>
        public bool CanTransitionTo(OrderStatus newStatus)
        {
            return Status switch
            {
                OrderStatus.Pending => newStatus is OrderStatus.Processing or OrderStatus.Cancelled,
                OrderStatus.Processing => newStatus is OrderStatus.Shipped or OrderStatus.Cancelled,
                OrderStatus.Shipped => newStatus is OrderStatus.Delivered or OrderStatus.Returned,
                OrderStatus.Delivered => newStatus is OrderStatus.Completed or OrderStatus.Returned,
                _ => false
            };
        }

        /// <summary>
        /// Sipariş durumunu değiştirir; geçersiz geçişte InvalidOperationException fırlatır.
        /// </summary>
        public void ChangeStatus(OrderStatus newStatus)
        {
            if (!CanTransitionTo(newStatus))
            {
                throw new InvalidOperationException($"Sipariş durumu {Status} durumundan {newStatus} durumuna geçirilemez.");
            }

            Status = newStatus;
        }
    }
}
