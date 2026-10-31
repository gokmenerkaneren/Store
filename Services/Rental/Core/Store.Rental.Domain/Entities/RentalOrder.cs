using System;
using System.Collections.Generic;
using Store.Rental.Domain.Enums;
using Store.Shared.Entities;

namespace Store.Rental.Domain.Entities
{
    public class RentalOrder : BaseEntity
    {
        public Guid UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public RentalStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public ICollection<RentalOrderItem> RentalOrderItems { get; set; }
        
        public RentalOrder()
        {
            RentalOrderItems = new HashSet<RentalOrderItem>();
        }
    }
}
