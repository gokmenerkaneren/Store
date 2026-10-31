namespace Store.Location.Application.Features.CQRS.Commands.CityCommands
{
    public class SoftDeleteCityCommand
    {
        public Guid Id { get; set; }
        public SoftDeleteCityCommand(Guid id) => Id = id;
    }
}
