namespace Store.Location.Application.Features.CQRS.Commands.ContactInfoCommands
{
    public class RemoveContactInfoCommand
    {
        public Guid Id { get; set; }
        public RemoveContactInfoCommand(Guid id) => Id = id;
    }
}
