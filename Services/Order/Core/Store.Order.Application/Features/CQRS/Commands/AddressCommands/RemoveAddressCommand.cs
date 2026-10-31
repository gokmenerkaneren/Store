using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Order.Application.Features.CQRS.Commands.AddressCommands
{
    public class RemoveAddressCommand
    {
        public Guid Id { get; set; }
        public RemoveAddressCommand(Guid id)
        {
            Id = id;
        }
    }
}


