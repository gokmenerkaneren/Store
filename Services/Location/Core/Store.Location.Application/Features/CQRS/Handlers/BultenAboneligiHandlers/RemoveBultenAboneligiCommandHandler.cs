using Store.Location.Application.Features.CQRS.Commands.BultenAboneligiCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.BultenAboneligiHandlers
{
    public class RemoveBultenAboneligiCommandHandler
    {
        private readonly IRepository<BultenAboneligi> _repository;

        public RemoveBultenAboneligiCommandHandler(IRepository<BultenAboneligi> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveBultenAboneligiCommand command)
        {
            var values = await _repository.GetByIdAsync(command.Id);
            if (values != null)
            {
                await _repository.SoftDeleteAsync(values);
            }
        }
    }
}
