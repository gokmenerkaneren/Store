namespace Store.Location.Application.Features.CQRS.Commands.NeighborhoodCommands
{
    public class SoftDeleteNeighborhoodCommand
    {
        public Guid Id { get; set; }
        public SoftDeleteNeighborhoodCommand(Guid id) => Id = id;
    }
}
