namespace Store.Location.Application.Features.CQRS.Commands.ContactInfoCommands
{
    public class DeleteContactInfoCommand
    {
        public Guid Id { get; set; }
        public DeleteContactInfoCommand(Guid id) => Id = id;
    }
}
