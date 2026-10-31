using Store.Location.Application.Features.CQRS.Commands.WarehouseCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.WarehouseHandlers
{
    public class SoftDeleteWarehouseCommandHandler
    {
        private readonly IRepository<Warehouse> _repository;

        public SoftDeleteWarehouseCommandHandler(IRepository<Warehouse> repository)
        {
            _repository = repository;
        }

        public async Task Handle(SoftDeleteWarehouseCommand command)
        {
            var warehouse = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (warehouse == null) return;

            if (warehouse.IsDefault)
            {
                throw new InvalidOperationException("Varsayılan depo silinemez.");
            }

            await _repository.SoftDeleteAsync(warehouse);
        }
    }
}
