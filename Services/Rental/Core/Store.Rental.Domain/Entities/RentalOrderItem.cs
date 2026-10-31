using System;
using Store.Shared.Entities;

namespace Store.Rental.Domain.Entities
{
    public class RentalOrderItem : BaseEntity
    {
        public Guid RentalOrderId { get; set; }
        public RentalOrder RentalOrder { get; set; }
        
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// İ4: PostgreSQL EXCLUDE constraint için tarih aralığı alanları
        /// </summary>
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
