using Store.Location.Application.Features.CQRS.Commands.NeighborhoodCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.NeighborhoodHandlers
{
    public class RemoveNeighborhoodCommandHandler
    {
        private readonly IRepository<Neighborhood> _repository;

        public RemoveNeighborhoodCommandHandler(IRepository<Neighborhood> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveNeighborhoodCommand command)
        {
            var neighborhood = await _repository.GetByIdAsync(command.Id);
            if (neighborhood != null)
            {
                await _repository.SoftDeleteAsync(neighborhood);
            }
        }
    }
}
