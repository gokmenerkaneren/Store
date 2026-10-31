using System;

namespace Store.Location.Application.Features.CQRS.Commands.UserAddressCommands
{
    public class SetDefaultUserAddressCommand
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }

        public SetDefaultUserAddressCommand(Guid id, Guid customerId)
        {
            Id = id;
            CustomerId = customerId;
        }
    }
}

