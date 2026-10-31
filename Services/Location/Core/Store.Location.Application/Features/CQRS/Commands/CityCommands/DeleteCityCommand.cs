namespace Store.Location.Application.Features.CQRS.Commands.CityCommands
{
    public class DeleteCityCommand
    {
        public Guid Id { get; set; }
        public DeleteCityCommand(Guid id) => Id = id;
    }
}
