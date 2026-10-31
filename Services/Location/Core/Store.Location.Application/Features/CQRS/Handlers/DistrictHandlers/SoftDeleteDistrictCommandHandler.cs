using Store.Location.Application.Features.CQRS.Commands.DistrictCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.DistrictHandlers
{
    public class SoftDeleteDistrictCommandHandler
    {
        private readonly IRepository<District> _repository;

        public SoftDeleteDistrictCommandHandler(IRepository<District> repository)
        {
            _repository = repository;
        }

        public async Task Handle(SoftDeleteDistrictCommand command)
        {
            var district = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (district == null) return;

            await _repository.SoftDeleteAsync(district);
        }
    }
}
