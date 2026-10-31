using Store.Location.Application.Features.CQRS.Commands.WarehouseCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.WarehouseHandlers
{
    public class RemoveWarehouseCommandHandler
    {
        private readonly IRepository<Warehouse> _repository;

        public RemoveWarehouseCommandHandler(IRepository<Warehouse> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveWarehouseCommand command)
        {
            var warehouse = await _repository.GetByIdAsync(command.Id);
            if (warehouse != null)
            {
                if (warehouse.IsDefault)
                {
                    throw new InvalidOperationException("Varsayılan depo silinemez.");
                }
                await _repository.SoftDeleteAsync(warehouse);
            }
        }
    }
}
