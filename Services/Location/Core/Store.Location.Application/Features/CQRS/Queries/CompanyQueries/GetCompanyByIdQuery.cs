namespace Store.Location.Application.Features.CQRS.Queries.CompanyQueries
{
    public class GetCompanyByIdQuery
    {
        public Guid Id { get; set; }
        public GetCompanyByIdQuery(Guid id) => Id = id;
    }
}
