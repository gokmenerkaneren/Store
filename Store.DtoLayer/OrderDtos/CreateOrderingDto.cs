using System;

namespace Store.DtoLayer.OrderDtos
{
    public class CreateOrderingDto
    {
        public Guid UserId { get; set; }
        public decimal TotalPrice { get; set; } = 0;
        public DateTime OrderDate { get; set; } = DateTime.Now;
    }
}

