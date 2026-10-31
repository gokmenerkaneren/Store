namespace Store.Location.Application.Features.CQRS.Commands.BranchAddressCommands
{
    public class RemoveBranchAddressCommand
    {
        public Guid Id { get; set; }
        public RemoveBranchAddressCommand(Guid id) => Id = id;
    }
}
