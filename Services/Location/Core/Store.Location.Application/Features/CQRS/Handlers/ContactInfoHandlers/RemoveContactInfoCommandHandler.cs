using Store.Location.Application.Features.CQRS.Commands.ContactInfoCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.ContactInfoHandlers
{
    public class RemoveContactInfoCommandHandler
    {
        private readonly IRepository<ContactInfo> _repository;

        public RemoveContactInfoCommandHandler(IRepository<ContactInfo> repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveContactInfoCommand command)
        {
            var contactInfo = await _repository.GetByIdAsync(command.Id);
            if (contactInfo != null)
            {
                await _repository.SoftDeleteAsync(contactInfo);
            }
        }
    }
}
