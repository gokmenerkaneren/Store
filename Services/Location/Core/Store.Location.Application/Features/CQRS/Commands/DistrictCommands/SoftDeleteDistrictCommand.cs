namespace Store.Location.Application.Features.CQRS.Commands.DistrictCommands
{
    public class SoftDeleteDistrictCommand
    {
        public Guid Id { get; set; }
        public SoftDeleteDistrictCommand(Guid id) => Id = id;
    }
}
