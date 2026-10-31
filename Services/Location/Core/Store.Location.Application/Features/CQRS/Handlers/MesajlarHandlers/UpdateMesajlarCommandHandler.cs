using Store.Location.Application.Features.CQRS.Commands.MesajlarCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.MesajlarHandlers
{
    public class UpdateMesajlarCommandHandler
    {
        private readonly IRepository<Mesajlar> _repository;

        public UpdateMesajlarCommandHandler(IRepository<Mesajlar> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateMesajlarCommand command)
        {
            var values = await _repository.GetByIdAsync(command.Id);
            if (values != null)
            {
                values.IsRead = command.IsRead;
                await _repository.UpdateAsync(values);
            }
        }
    }
}
