namespace Store.Location.Application.Features.CQRS.Commands.BranchCommands
{
    public class DeleteBranchCommand
    {
        public Guid Id { get; set; }
        public DeleteBranchCommand(Guid id) => Id = id;
    }
}
