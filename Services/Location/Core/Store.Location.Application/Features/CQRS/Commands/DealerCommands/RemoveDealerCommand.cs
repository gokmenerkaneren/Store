namespace Store.Location.Application.Features.CQRS.Commands.DealerCommands
{
    public class RemoveDealerCommand
    {
        public Guid Id { get; set; }
        public RemoveDealerCommand(Guid id) => Id = id;
    }
}
