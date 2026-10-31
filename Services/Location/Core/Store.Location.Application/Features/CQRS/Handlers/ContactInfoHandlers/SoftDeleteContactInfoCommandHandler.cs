using Store.Location.Application.Features.CQRS.Commands.ContactInfoCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.ContactInfoHandlers
{
    public class SoftDeleteContactInfoCommandHandler
    {
        private readonly IRepository<ContactInfo> _repository;

        public SoftDeleteContactInfoCommandHandler(IRepository<ContactInfo> repository)
        {
            _repository = repository;
        }

        public async Task Handle(SoftDeleteContactInfoCommand command)
        {
            var contactInfo = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (contactInfo == null) return;

            await _repository.SoftDeleteAsync(contactInfo);
        }
    }
}
