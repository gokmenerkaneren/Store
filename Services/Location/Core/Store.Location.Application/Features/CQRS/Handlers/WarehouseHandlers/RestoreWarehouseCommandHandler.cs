using Store.Location.Application.Features.CQRS.Commands.WarehouseCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.WarehouseHandlers
{
    public class RestoreWarehouseCommandHandler
    {
        private readonly IRepository<Warehouse> _repository;

        public RestoreWarehouseCommandHandler(IRepository<Warehouse> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RestoreWarehouseCommand command)
        {
            var warehouse = await _repository.GetByIdAsync(command.Id);
            if (warehouse == null) return;

            await _repository.RestoreAsync(warehouse);
        }
    }
}
