namespace Store.Location.Application.Features.CQRS.Commands.CityCommands
{
    public class RemoveCityCommand
    {
        public Guid Id { get; set; }
        public RemoveCityCommand(Guid id) => Id = id;
    }
}
