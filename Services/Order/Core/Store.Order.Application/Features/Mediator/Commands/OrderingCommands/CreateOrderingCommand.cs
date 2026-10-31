using MediatR;
using Store.Order.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Store.Order.Application.Features.Mediator.Commands.OrderingCommands
{
    public class CreateOrderingCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public decimal TotalPrice { get; set; } = 0;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        /// <summary>
        /// İ2: Siparişe ait kalemler (OrderDetail)
        /// </summary>
        public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
    }

    public class CreateOrderItemDto
    {
        public Guid ProductId { get; set; }
        public decimal ProductPrice { get; set; } = 0;
        public int ProductAmount { get; set; } = 0;
        public decimal TotalPrice { get; set; } = 0;
        public Guid PriceTypeId { get; set; } = Guid.Empty;
        public decimal OriginalPrice { get; set; } = 0;
        public decimal DiscountRateApplied { get; set; } = 0;
        public Guid CurrencyId { get; set; } = Guid.Empty;
    }
}
