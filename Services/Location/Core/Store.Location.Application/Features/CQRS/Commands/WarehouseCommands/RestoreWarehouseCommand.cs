using System;

namespace Store.Location.Application.Features.CQRS.Commands.WarehouseCommands
{
    public class RestoreWarehouseCommand
    {
        public Guid Id { get; set; }
        public RestoreWarehouseCommand(Guid id) => Id = id;
    }
}
