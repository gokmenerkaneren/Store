using System;

namespace Store.Location.Application.Features.CQRS.Commands.NeighborhoodCommands
{
    public class RestoreNeighborhoodCommand
    {
        public Guid Id { get; set; }
        public RestoreNeighborhoodCommand(Guid id) => Id = id;
    }
}
