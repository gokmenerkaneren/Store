using Store.Location.Application.Features.CQRS.Commands.NeighborhoodCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.NeighborhoodHandlers
{
    public class DeleteNeighborhoodCommandHandler
    {
        private readonly IRepository<Neighborhood> _repository;

        public DeleteNeighborhoodCommandHandler(IRepository<Neighborhood> repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteNeighborhoodCommand command)
        {
            var neighborhood = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (neighborhood == null) return;

            await _repository.SoftDeleteAsync(neighborhood);
        }
    }
}
