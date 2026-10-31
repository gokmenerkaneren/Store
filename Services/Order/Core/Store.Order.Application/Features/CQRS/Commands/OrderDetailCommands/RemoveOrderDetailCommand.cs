using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Order.Application.Features.CQRS.Commands.OrderDetailCommands
{
    public class RemoveOrderDetailCommand
    {
        public Guid Id { get; set; }
        public RemoveOrderDetailCommand(Guid id)
        {
            Id = id;
        }
    }
}


