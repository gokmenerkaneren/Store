namespace Store.Location.Application.Features.CQRS.Commands.DistrictCommands
{
    public class RemoveDistrictCommand
    {
        public Guid Id { get; set; }
        public RemoveDistrictCommand(Guid id) => Id = id;
    }
}
