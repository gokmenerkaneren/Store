using Store.Location.Application.Features.CQRS.Commands.MesajlarCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.MesajlarHandlers
{
    public class CreateMesajlarCommandHandler
    {
        private readonly IRepository<Mesajlar> _repository;

        public CreateMesajlarCommandHandler(IRepository<Mesajlar> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateMesajlarCommand command)
        {
            await _repository.CreateAsync(new Mesajlar
            {
                NameSurname = command.NameSurname,
                Email = command.Email,
                Phone = command.Phone,
                Subject = command.Subject,
                Message = command.Message,
                IsRead = false,
                MessageDate = DateTime.UtcNow
            });
        }
    }
}
