namespace Store.Location.Application.Features.CQRS.Commands.UserAddressCommands
{
    public class SoftDeleteUserAddressCommand
    {
        public Guid Id { get; set; }
        public SoftDeleteUserAddressCommand(Guid id) => Id = id;
    }
}
