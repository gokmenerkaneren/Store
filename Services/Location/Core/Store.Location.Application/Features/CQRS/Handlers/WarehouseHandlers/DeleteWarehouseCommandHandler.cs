using Store.Location.Application.Features.CQRS.Commands.WarehouseCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.WarehouseHandlers
{
    public class DeleteWarehouseCommandHandler
    {
        private readonly IRepository<Warehouse> _repository;

        public DeleteWarehouseCommandHandler(IRepository<Warehouse> repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteWarehouseCommand command)
        {
            var warehouse = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (warehouse == null) return;

            await _repository.SoftDeleteAsync(warehouse);
        }
    }
}
