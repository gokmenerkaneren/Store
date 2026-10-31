using Store.Location.Application.Features.CQRS.Commands.CityCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.CityHandlers
{
    public class SoftDeleteCityCommandHandler
    {
        private readonly IRepository<City> _repository;

        public SoftDeleteCityCommandHandler(IRepository<City> repository)
        {
            _repository = repository;
        }

        public async Task Handle(SoftDeleteCityCommand command)
        {
            var city = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (city == null) return;

            await _repository.SoftDeleteAsync(city);
        }
    }
}
