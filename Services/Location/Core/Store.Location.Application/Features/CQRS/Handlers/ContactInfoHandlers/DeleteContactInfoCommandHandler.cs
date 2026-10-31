using Store.Location.Application.Features.CQRS.Commands.ContactInfoCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.ContactInfoHandlers
{
    public class DeleteContactInfoCommandHandler
    {
        private readonly IRepository<ContactInfo> _repository;

        public DeleteContactInfoCommandHandler(IRepository<ContactInfo> repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteContactInfoCommand command)
        {
            var contactInfo = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (contactInfo == null) return;

            await _repository.SoftDeleteAsync(contactInfo);
        }
    }
}
