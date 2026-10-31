namespace Store.B2B.Application.Features.CQRS.Commands.DealerFinancialCommands
{
    public class RemoveDealerFinancialCommand
    {
        public Guid Id { get; set; }

        public RemoveDealerFinancialCommand(Guid id)
        {
            Id = id;
        }
    }
}
