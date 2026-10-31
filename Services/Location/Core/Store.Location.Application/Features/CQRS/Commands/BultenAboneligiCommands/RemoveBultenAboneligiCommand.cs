using System;

namespace Store.Location.Application.Features.CQRS.Commands.BultenAboneligiCommands
{
    public class RemoveBultenAboneligiCommand
    {
        public Guid Id { get; set; }
        public RemoveBultenAboneligiCommand(Guid id)
        {
            Id = id;
        }
    }
}
