namespace Store.Location.Application.Features.CQRS.Commands.BranchCommands
{
    public class RemoveBranchCommand
    {
        public Guid Id { get; set; }
        public RemoveBranchCommand(Guid id) => Id = id;
    }
}
