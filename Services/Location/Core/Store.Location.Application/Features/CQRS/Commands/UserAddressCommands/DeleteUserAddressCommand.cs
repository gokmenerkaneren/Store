namespace Store.Location.Application.Features.CQRS.Commands.UserAddressCommands
{
    public class DeleteUserAddressCommand
    {
        public Guid Id { get; set; }
        public DeleteUserAddressCommand(Guid id) => Id = id;
    }
}
