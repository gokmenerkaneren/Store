namespace Store.Location.Application.Features.CQRS.Commands.CountryCommands
{
    public class RemoveCountryCommand
    {
        public Guid Id { get; set; }
        public RemoveCountryCommand(Guid id) => Id = id;
    }
}
