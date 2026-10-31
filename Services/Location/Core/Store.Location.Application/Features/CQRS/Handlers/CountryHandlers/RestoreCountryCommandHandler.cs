using Store.Location.Application.Features.CQRS.Commands.CountryCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.CountryHandlers
{
    public class RestoreCountryCommandHandler
    {
        private readonly IRepository<Country> _repository;

        public RestoreCountryCommandHandler(IRepository<Country> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RestoreCountryCommand command)
        {
            // Silinmiş kaydı IgnoreQueryFilters ile bulan GetByIdAsync kullanılıyor
            var country = await _repository.GetByIdAsync(command.Id);
            if (country == null) return;

            await _repository.RestoreAsync(country);
        }
    }
}
