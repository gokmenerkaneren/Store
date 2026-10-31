using Store.Location.Application.Features.CQRS.Commands.ContactInfoCommands;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.ContactInfoHandlers
{
    public class UpdateContactInfoCommandHandler
    {
        private readonly IRepository<ContactInfo> _repository;

        public UpdateContactInfoCommandHandler(IRepository<ContactInfo> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateContactInfoCommand command)
        {
            var contactInfo = await _repository.GetByFilterAsync(x => x.Id == command.Id);
            if (contactInfo == null) return;

            contactInfo.CompanyEmail = command.CompanyEmail;
            contactInfo.Phone = command.Phone;
            contactInfo.Fax = command.Fax;
            contactInfo.Website = command.Website;
            contactInfo.AuthorizedPersonName = command.AuthorizedPersonName;
            contactInfo.AuthorizedPersonTitle = command.AuthorizedPersonTitle;
            contactInfo.AuthorizedPersonPhone = command.AuthorizedPersonPhone;
            contactInfo.AuthorizedPersonEmail = command.AuthorizedPersonEmail;
            contactInfo.IsActive = command.IsActive;
            contactInfo.Notes = command.Notes;
            contactInfo.UpdateUserId = Guid.Parse("00000002-0002-0002-0000-000000000002");
            contactInfo.UpdateDate = BaseEntity.GetLocalTime();

            await _repository.UpdateAsync(contactInfo);
        }
    }
}
