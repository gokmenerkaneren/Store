using Store.Location.Application.Features.CQRS.Commands.CountryCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.CountryHandlers
{
    public class SoftDeleteCountryCommandHandler
    {
        private readonly IRepository<Country> _repository;

        public SoftDeleteCountryCommandHandler(IRepository<Country> repository)
        {
            _repository = repository;
        }

        public async Task Handle(SoftDeleteCountryCommand command)
        {
            var country = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (country == null) return;

            await _repository.SoftDeleteAsync(country);
        }
    }
}
