using Store.Location.Application.Features.CQRS.Commands.MesajlarCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.MesajlarHandlers
{
    public class RemoveMesajlarCommandHandler
    {
        private readonly IRepository<Mesajlar> _repository;

        public RemoveMesajlarCommandHandler(IRepository<Mesajlar> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveMesajlarCommand command)
        {
            var values = await _repository.GetByIdAsync(command.Id);
            if (values != null)
            {
                await _repository.SoftDeleteAsync(values);
            }
        }
    }
}
