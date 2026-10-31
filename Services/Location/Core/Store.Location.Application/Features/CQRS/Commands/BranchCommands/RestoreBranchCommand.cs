using System;

namespace Store.Location.Application.Features.CQRS.Commands.BranchCommands
{
    public class RestoreBranchCommand
    {
        public Guid Id { get; set; }
        public RestoreBranchCommand(Guid id) => Id = id;
    }
}
