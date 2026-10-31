namespace Store.Location.Application.Features.CQRS.Commands.BranchCommands
{
    public class SoftDeleteBranchCommand
    {
        public Guid Id { get; set; }
        public SoftDeleteBranchCommand(Guid id) => Id = id;
    }
}
