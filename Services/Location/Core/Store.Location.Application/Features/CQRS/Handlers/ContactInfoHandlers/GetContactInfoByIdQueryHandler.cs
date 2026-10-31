using Store.Location.Application.Features.CQRS.Queries.ContactInfoQueries;
using Store.Location.Application.Features.CQRS.Results.ContactInfoResults;
using Store.Location.Application.Interfaces;
using Store.Location.Domain.Entities;

namespace Store.Location.Application.Features.CQRS.Handlers.ContactInfoHandlers
{
    public class GetContactInfoByIdQueryHandler
    {
        private readonly IRepository<ContactInfo> _repository;

        public GetContactInfoByIdQueryHandler(IRepository<ContactInfo> repository)
        {
            _repository = repository;
        }

        public async Task<GetContactInfoByIdQueryResult?> Handle(GetContactInfoByIdQuery query)
        {
            var values = await _repository.GetByFilterAsync(x => x.Id == query.Id);
            if (values == null) return null;

            return new GetContactInfoByIdQueryResult
            {
                Id = values.Id,
                CompanyEmail = values.CompanyEmail,
                Phone = values.Phone,
                Fax = values.Fax,
                Website = values.Website,
                AuthorizedPersonName = values.AuthorizedPersonName,
                AuthorizedPersonTitle = values.AuthorizedPersonTitle,
                AuthorizedPersonPhone = values.AuthorizedPersonPhone,
                AuthorizedPersonEmail = values.AuthorizedPersonEmail,
                IsActive = values.IsActive,
                IsDeleted = values.IsDeleted,
                Notes = values.Notes,
                InsertDate = values.InsertDate,
                UpdateDate = values.UpdateDate,
                InsertUserId = values.InsertUserId,
                UpdateUserId = values.UpdateUserId,
                DeleteUserId = values.DeleteUserId,
                DeleteDate = values.DeleteDate,
            };
        }
    }
}
