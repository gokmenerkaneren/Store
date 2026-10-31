namespace Store.Location.Application.Features.CQRS.Commands.WarehouseCommands
{
    public class RemoveWarehouseCommand
    {
        public Guid Id { get; set; }
        public RemoveWarehouseCommand(Guid id) => Id = id;
    }
}
