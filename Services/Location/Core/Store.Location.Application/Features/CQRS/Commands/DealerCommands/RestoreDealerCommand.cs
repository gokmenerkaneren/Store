using System;

namespace Store.Location.Application.Features.CQRS.Commands.DealerCommands
{
    public class RestoreDealerCommand
    {
        public Guid Id { get; set; }
        public RestoreDealerCommand(Guid id) => Id = id;
    }
}
