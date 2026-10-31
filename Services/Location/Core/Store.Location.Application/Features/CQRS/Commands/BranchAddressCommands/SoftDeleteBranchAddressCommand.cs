namespace Store.Location.Application.Features.CQRS.Commands.BranchAddressCommands
{
    public class SoftDeleteBranchAddressCommand
    {
        public Guid Id { get; set; }
        public SoftDeleteBranchAddressCommand(Guid id) => Id = id;
    }
}
