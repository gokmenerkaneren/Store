using MediatR;
using System;
using System.Collections.Generic;

namespace Store.Rental.Application.Features.RentalOrders.Commands.CreateRentalOrder
{
    public class CreateRentalOrderCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<RentalOrderItemDto> Items { get; set; }
    }

    public class RentalOrderItemDto
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
