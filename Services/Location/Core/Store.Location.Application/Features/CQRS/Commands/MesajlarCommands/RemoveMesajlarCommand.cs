using System;

namespace Store.Location.Application.Features.CQRS.Commands.MesajlarCommands
{
    public class RemoveMesajlarCommand
    {
        public Guid Id { get; set; }
        public RemoveMesajlarCommand(Guid id)
        {
            Id = id;
        }
    }
}
