using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Order.Application.Features.Mediator.Commands.OrderingCommands
{
    public class RemoveOrderingCommand:IRequest
    {
        public Guid Id { get; set; }
        public RemoveOrderingCommand(Guid id)
        {
            Id = id;
        }
    }
}


