namespace Store.Location.Application.Features.CQRS.Queries.CityQueries
{
    public class GetCityByIdQuery
    {
        public Guid Id { get; set; }
        public GetCityByIdQuery(Guid id) => Id = id;
    }
}
