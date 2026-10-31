using Store.Location.Application.Features.CQRS.Commands.BultenAboneligiCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.BultenAboneligiHandlers
{
    public class UpdateBultenAboneligiCommandHandler
    {
        private readonly IRepository<BultenAboneligi> _repository;

        public UpdateBultenAboneligiCommandHandler(IRepository<BultenAboneligi> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateBultenAboneligiCommand command)
        {
            var values = await _repository.GetByIdAsync(command.Id);
            if (values != null)
            {
                values.Email = command.Email;
                await _repository.UpdateAsync(values);
            }
        }
    }
}
