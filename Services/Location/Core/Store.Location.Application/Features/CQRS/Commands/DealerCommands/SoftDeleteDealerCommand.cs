namespace Store.Location.Application.Features.CQRS.Commands.DealerCommands
{
    public class SoftDeleteDealerCommand
    {
        public Guid Id { get; set; }
        public SoftDeleteDealerCommand(Guid id) => Id = id;
    }
}
