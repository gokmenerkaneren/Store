namespace Store.Location.Application.Features.CQRS.Commands.NeighborhoodCommands
{
    public class RemoveNeighborhoodCommand
    {
        public Guid Id { get; set; }
        public RemoveNeighborhoodCommand(Guid id) => Id = id;
    }
}
