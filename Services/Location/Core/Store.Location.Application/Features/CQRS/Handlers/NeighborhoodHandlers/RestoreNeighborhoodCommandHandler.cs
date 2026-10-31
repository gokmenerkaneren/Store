using Store.Location.Application.Features.CQRS.Commands.NeighborhoodCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.NeighborhoodHandlers
{
    public class RestoreNeighborhoodCommandHandler
    {
        private readonly IRepository<Neighborhood> _repository;

        public RestoreNeighborhoodCommandHandler(IRepository<Neighborhood> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RestoreNeighborhoodCommand command)
        {
            var neighborhood = await _repository.GetByIdAsync(command.Id);
            if (neighborhood == null) return;

            await _repository.RestoreAsync(neighborhood);
        }
    }
}
