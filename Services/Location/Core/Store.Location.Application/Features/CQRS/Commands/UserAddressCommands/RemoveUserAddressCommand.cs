namespace Store.Location.Application.Features.CQRS.Commands.UserAddressCommands
{
    public class RemoveUserAddressCommand
    {
        public Guid Id { get; set; }
        public RemoveUserAddressCommand(Guid id) => Id = id;
    }
}
