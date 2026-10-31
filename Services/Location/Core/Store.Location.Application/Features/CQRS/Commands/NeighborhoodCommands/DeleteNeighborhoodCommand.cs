namespace Store.Location.Application.Features.CQRS.Commands.NeighborhoodCommands
{
    public class DeleteNeighborhoodCommand
    {
        public Guid Id { get; set; }
        public DeleteNeighborhoodCommand(Guid id) => Id = id;
    }
}
