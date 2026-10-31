namespace Store.Location.Application.Features.CQRS.Commands.DealerAddressCommands
{
    public class DeleteDealerAddressCommand
    {
        public Guid Id { get; set; }
        public DeleteDealerAddressCommand(Guid id) => Id = id;
    }

    public class SoftDeleteDealerAddressCommand
    {
        public Guid Id { get; set; }
        public SoftDeleteDealerAddressCommand(Guid id) => Id = id;
    }

    public class RestoreDealerAddressCommand
    {
        public Guid Id { get; set; }
        public RestoreDealerAddressCommand(Guid id) => Id = id;
    }
}
