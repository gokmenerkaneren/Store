namespace Store.Location.Application.Features.CQRS.Commands.BranchAddressCommands
{
    public class DeleteBranchAddressCommand
    {
        public Guid Id { get; set; }
        public DeleteBranchAddressCommand(Guid id) => Id = id;
    }
}
