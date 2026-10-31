namespace Store.Location.Application.Features.CQRS.Queries.CountryQueries
{
    public class GetCountryByIdQuery
    {
        public Guid Id { get; set; }
        public GetCountryByIdQuery(Guid id) => Id = id;
    }
}
