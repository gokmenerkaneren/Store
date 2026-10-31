namespace Store.Location.Application.Features.CQRS.Queries.WarehouseQueries
{
    public class GetWarehouseByIdQuery
    {
        public Guid Id { get; set; }
        public GetWarehouseByIdQuery(Guid id) => Id = id;
    }
}
