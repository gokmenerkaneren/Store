using Store.Location.Application.Features.CQRS.Commands.CountryCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System.Net;

namespace Store.Location.Application.Features.CQRS.Handlers.CountryHandlers
{
    public class RemoveCountryCommandHandler
    {
        private readonly IRepository<Country> _repository;

        public RemoveCountryCommandHandler(IRepository<Country> repository)
        {
            _repository = repository;
        }
        public async Task Handle(RemoveCountryCommand command)
        {
            var country = await _repository.GetByIdAsync(command.Id);
            if (country != null)
            {
                await _repository.SoftDeleteAsync(country);
            }
        }
    }
}
