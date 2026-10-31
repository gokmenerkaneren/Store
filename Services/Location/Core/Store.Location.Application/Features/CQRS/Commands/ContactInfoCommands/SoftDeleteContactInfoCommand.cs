namespace Store.Location.Application.Features.CQRS.Commands.ContactInfoCommands
{
    public class SoftDeleteContactInfoCommand
    {
        public Guid Id { get; set; }
        public SoftDeleteContactInfoCommand(Guid id) => Id = id;
    }
}
