namespace Store.Location.Application.Features.CQRS.Commands.WarehouseCommands
{
    public class SoftDeleteWarehouseCommand
    {
        public Guid Id { get; set; }
        public SoftDeleteWarehouseCommand(Guid id) => Id = id;
    }
}
