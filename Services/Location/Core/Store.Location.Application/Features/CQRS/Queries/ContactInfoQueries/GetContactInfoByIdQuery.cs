namespace Store.Location.Application.Features.CQRS.Queries.ContactInfoQueries
{
    public class GetContactInfoByIdQuery
    {
        public Guid Id { get; set; }
        public GetContactInfoByIdQuery(Guid id) => Id = id;
    }
}
