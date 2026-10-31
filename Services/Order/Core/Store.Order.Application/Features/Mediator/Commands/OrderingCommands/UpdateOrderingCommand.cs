using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Order.Application.Features.Mediator.Commands.OrderingCommands
{
    public class UpdateOrderingCommand: IRequest
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal TotalPrice { get; set; } = 0;
        public DateTime OrderDate { get; set; } = DateTime.Now;
    }
}


