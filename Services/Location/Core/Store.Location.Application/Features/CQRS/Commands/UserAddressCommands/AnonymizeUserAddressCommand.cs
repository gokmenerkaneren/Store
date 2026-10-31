namespace Store.Location.Application.Features.CQRS.Commands.UserAddressCommands
{
    public class AnonymizeUserAddressCommand
    {
        public Guid Id { get; set; }
        public AnonymizeUserAddressCommand(Guid id) => Id = id;
    }
}
