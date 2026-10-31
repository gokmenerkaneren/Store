using Store.Location.Application.Features.CQRS.Commands.DistrictCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.DistrictHandlers
{
    public class RemoveDistrictCommandHandler
    {
        private readonly IRepository<District> _repository;

        public RemoveDistrictCommandHandler(IRepository<District> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveDistrictCommand command)
        {
            var district = await _repository.GetByIdAsync(command.Id);
            if (district != null)
            {
                await _repository.SoftDeleteAsync(district);
            }
        }
    }
}
