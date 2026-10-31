using Store.Location.Application.Features.CQRS.Commands.ContactInfoCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.ContactInfoHandlers
{
    public class CreateContactInfoCommandHandler
    {
        private readonly IRepository<ContactInfo> _repository;

        public CreateContactInfoCommandHandler(IRepository<ContactInfo> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateContactInfoCommand command)
        {
            var contactInfo = new ContactInfo
            {
                CompanyEmail = command.CompanyEmail,
                Phone = command.Phone,
                Fax = command.Fax,
                Website = command.Website,
                AuthorizedPersonName = command.AuthorizedPersonName,
                AuthorizedPersonTitle = command.AuthorizedPersonTitle,
                AuthorizedPersonPhone = command.AuthorizedPersonPhone,
                AuthorizedPersonEmail = command.AuthorizedPersonEmail,
                Notes = command.Notes,
                IsActive = true,
                IsDeleted = false,
                InsertUserId = Guid.Parse("00000001-0001-0001-0001-000000000001"),
                InsertDate = BaseEntity.GetLocalTime()
            };

            await _repository.CreateAsync(contactInfo);
        }
    }
}
