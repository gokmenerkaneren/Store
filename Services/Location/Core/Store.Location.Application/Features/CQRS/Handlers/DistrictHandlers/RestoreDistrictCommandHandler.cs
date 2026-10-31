using Store.Location.Application.Features.CQRS.Commands.DistrictCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.DistrictHandlers
{
    public class RestoreDistrictCommandHandler
    {
        private readonly IRepository<District> _repository;

        public RestoreDistrictCommandHandler(IRepository<District> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RestoreDistrictCommand command)
        {
            var district = await _repository.GetByIdAsync(command.Id);
            if (district == null) return;

            await _repository.RestoreAsync(district);
        }
    }
}
