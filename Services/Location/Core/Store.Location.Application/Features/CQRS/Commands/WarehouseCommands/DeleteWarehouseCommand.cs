namespace Store.Location.Application.Features.CQRS.Commands.WarehouseCommands
{
    public class DeleteWarehouseCommand
    {
        public Guid Id { get; set; }
        public DeleteWarehouseCommand(Guid id) => Id = id;
    }
}
