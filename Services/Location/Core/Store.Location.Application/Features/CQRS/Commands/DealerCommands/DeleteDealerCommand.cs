namespace Store.Location.Application.Features.CQRS.Commands.DealerCommands
{
    public class DeleteDealerCommand
    {
        public Guid Id { get; set; }
        public DeleteDealerCommand(Guid id) => Id = id;
    }
}
