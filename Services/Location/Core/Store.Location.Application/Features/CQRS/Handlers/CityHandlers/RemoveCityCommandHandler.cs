using Store.Location.Application.Features.CQRS.Commands.CityCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.CityHandlers
{
    public class RemoveCityCommandHandler
    {
        private readonly IRepository<City> _repository;

        public RemoveCityCommandHandler(IRepository<City> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveCityCommand command)
        {
            var city = await _repository.GetByIdAsync(command.Id);
            if (city != null)
            {
                await _repository.SoftDeleteAsync(city);
            }
        }
    }
}
