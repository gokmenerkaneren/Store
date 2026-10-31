using Store.Location.Application.Features.CQRS.Queries.ContactInfoQueries;
using Store.Location.Application.Features.CQRS.Results.ContactInfoResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.ContactInfoHandlers
{
    public class GetContactInfosQueryHandler
    {
        private readonly IRepository<ContactInfo> _repository;

        public GetContactInfosQueryHandler(IRepository<ContactInfo> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetContactInfoQueryResult>> Handle(bool showDeleted = false)
        {
            var contactInfos = await _repository.GetAllAsync(includeDeleted: showDeleted);
            if (!showDeleted) contactInfos = contactInfos.Where(x => !x.IsDeleted).ToList();
            return contactInfos.Select(x => new GetContactInfoQueryResult
            {
                Id = x.Id,
                CompanyEmail = x.CompanyEmail,
                Phone = x.Phone,
                Fax = x.Fax,
                Website = x.Website,
                AuthorizedPersonName = x.AuthorizedPersonName,
                AuthorizedPersonTitle = x.AuthorizedPersonTitle,
                AuthorizedPersonPhone = x.AuthorizedPersonPhone,
                AuthorizedPersonEmail = x.AuthorizedPersonEmail,
                IsActive = x.IsActive,
                Notes = x.Notes,
                InsertUserId = x.InsertUserId,
                InsertDate = x.InsertDate,
                UpdateUserId = x.UpdateUserId,
                UpdateDate = x.UpdateDate,
                IsDeleted = x.IsDeleted,
                DeleteUserId = x.DeleteUserId,
                DeleteDate = x.DeleteDate,
            }).ToList();
        }
    }
}
