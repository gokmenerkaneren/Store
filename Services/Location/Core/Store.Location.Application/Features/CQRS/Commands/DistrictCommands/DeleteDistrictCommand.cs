namespace Store.Location.Application.Features.CQRS.Commands.DistrictCommands
{
    public class DeleteDistrictCommand
    {
        public Guid Id { get; set; }
        public DeleteDistrictCommand(Guid id) => Id = id;
    }
}
