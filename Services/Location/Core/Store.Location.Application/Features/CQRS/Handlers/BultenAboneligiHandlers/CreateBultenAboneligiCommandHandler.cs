using Store.Location.Application.Features.CQRS.Commands.BultenAboneligiCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Store.Location.Application.Features.CQRS.Handlers.BultenAboneligiHandlers
{
    public class CreateBultenAboneligiCommandHandler
    {
        private readonly IRepository<BultenAboneligi> _repository;

        public CreateBultenAboneligiCommandHandler(IRepository<BultenAboneligi> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateBultenAboneligiCommand command)
        {
            await _repository.CreateAsync(new BultenAboneligi
            {
                Email = command.Email
            });
        }
    }
}
