using Store.Location.Application.Features.CQRS.Commands.CityCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.CityHandlers
{
    public class RestoreCityCommandHandler
    {
        private readonly IRepository<City> _repository;

        public RestoreCityCommandHandler(IRepository<City> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RestoreCityCommand command)
        {
            var city = await _repository.GetByIdAsync(command.Id);
            if (city == null) return;

            await _repository.RestoreAsync(city);
        }
    }
}
